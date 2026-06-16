using Npgsql;
namespace ProjectDTS;

public class RoleService
{
    
    private readonly DatabaseService _db;
    private readonly PermissionChangeLogService _permissionChangeLogService;
    private static readonly Dictionary<UserRole, HashSet<Permission>> _builtInPermissions = new()
    {
        [UserRole.SuperAdmin]   = new(Enum.GetValues<Permission>()),
        [UserRole.ProductAdmin] = new() { Permission.ManageProducts },
        [UserRole.OrderAdmin]   = new() { Permission.ManageOrders },
        [UserRole.UserAdmin]    = new() { Permission.ManageUsers, Permission.ManageReviews },
        [UserRole.Customer]     = new(),
    };

    public RoleService(DatabaseService db, PermissionChangeLogService permissionChangeLogService)
    {
        _db = db;
        _permissionChangeLogService = permissionChangeLogService;
    }

    public HashSet<Permission> GetPermissionsForUser(User user)
    {
        if (user.Role != UserRole.Custom)
            return _builtInPermissions.TryGetValue(user.Role, out var perms) ? new(perms) : new();

        return GetPermissionsForRoleName(user.CustomRoleName!);
    }

    private HashSet<Permission> GetPermissionsForRoleName(string roleName)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        const string sql = @"SELECT rp.permission FROM role_permissions rp
                             JOIN roles r ON r.id = rp.role_id
                             WHERE r.name = @name";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("name", roleName);

        var result = new HashSet<Permission>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            if (Enum.TryParse<Permission>(reader.GetString(0), true, out var perm))
                result.Add(perm);
        }
        return result;
    }

    public List<Role> GetAllRoles()
    {
        using var conn = _db.GetConnection();
        conn.Open();

        const string sql = @"SELECT r.id, r.name, r.is_builtin, rp.permission
                             FROM roles r
                             LEFT JOIN role_permissions rp ON rp.role_id = r.id
                             ORDER BY r.is_builtin DESC, r.name";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        var roles = new Dictionary<int, Role>();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            if (!roles.TryGetValue(id, out var role))
            {
                role = new Role
                {
                    Id = id,
                    Name = reader.GetString(1),
                    IsBuiltIn = reader.GetBoolean(2),
                };
                roles[id] = role;
            }
            if (!reader.IsDBNull(3) && Enum.TryParse<Permission>(reader.GetString(3), true, out var perm))
                role.Permissions.Add(perm);
        }
        return roles.Values.ToList();
    }

    public bool RoleNameExists(string name)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT 1 FROM roles WHERE LOWER(name) = LOWER(@name)", conn);
        cmd.Parameters.AddWithValue("name", name);
        return cmd.ExecuteScalar() != null;
    }

    public bool RoleHasUsers(int roleId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        const string sql = @"SELECT 1 FROM users u
                             JOIN roles r ON LOWER(u.role) = LOWER(r.name)
                             WHERE r.id = @roleId LIMIT 1";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("roleId", roleId);
        return cmd.ExecuteScalar() != null;
    }

    public bool CreateRole(string name, HashSet<Permission> permissions)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        var committed = false;

        try
        {
            int roleId;
            using (var cmd = new NpgsqlCommand(
                "INSERT INTO roles (name, is_builtin) VALUES (@name, false) RETURNING id", conn, tx))
            {
                cmd.Parameters.AddWithValue("name", name);
                roleId = (int)cmd.ExecuteScalar()!;
            }

            InsertPermissions(conn, tx, roleId, permissions);
            tx.Commit();
            committed = true;

            LogPermissionChange("RoleCreated", roleId, name, new HashSet<Permission>(), permissions);

            return true;
        }
        catch
        {
            if (!committed)
                tx.Rollback();
            return false;
        }
    }

    public bool UpdateRolePermissions(int roleId, HashSet<Permission> permissions)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        var role = GetAllRoles().First(r => r.Id == roleId);
        var oldPermissions = new HashSet<Permission>(role.Permissions);
        var committed = false;

        try
        {
            using (var del = new NpgsqlCommand("DELETE FROM role_permissions WHERE role_id = @id", conn, tx))
            {
                del.Parameters.AddWithValue("id", roleId);
                del.ExecuteNonQuery();
            }

                InsertPermissions(conn, tx, roleId, permissions);
                tx.Commit();
                committed = true;

                LogPermissionChange("PermissionChanged", role.Id, role.Name, oldPermissions, permissions);

                return true;
        }
        catch
        {
            if (!committed)
                tx.Rollback();
            return false;
        }
    }

    public bool DeleteRole(int roleId)
    {
        if (RoleHasUsers(roleId)) return false;

        var role = GetAllRoles().FirstOrDefault(r => r.Id == roleId && !r.IsBuiltIn);
        if (role == null) return false;

        using var conn = _db.GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM roles WHERE id = @id AND is_builtin = false", conn);
        cmd.Parameters.AddWithValue("id", roleId);
        var deleted = cmd.ExecuteNonQuery() > 0;

        if (deleted)
            LogPermissionChange("RoleDeleted", role.Id, role.Name, new HashSet<Permission>(role.Permissions), new HashSet<Permission>());

        return deleted;
    }

    public List<string> GetAllRoleNames()
    {
        using var conn = _db.GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT name FROM roles ORDER BY is_builtin DESC, name", conn);
        using var reader = cmd.ExecuteReader();
        var names = new List<string>();
        while (reader.Read()) names.Add(reader.GetString(0));
        return names;
    }

    private static void InsertPermissions(NpgsqlConnection conn, NpgsqlTransaction tx, int roleId, HashSet<Permission> permissions)
    {
        foreach (var perm in permissions)
        {
            using var ins = new NpgsqlCommand(
                "INSERT INTO role_permissions (role_id, permission) VALUES (@rid, @perm)", conn, tx);
            ins.Parameters.AddWithValue("rid", roleId);
            ins.Parameters.AddWithValue("perm", perm.ToString());
            ins.ExecuteNonQuery();
        }
    }

    private void LogPermissionChange(
        string actionType,
        int roleId,
        string roleName,
        HashSet<Permission> oldPermissions,
        HashSet<Permission> newPermissions)
    {
        _permissionChangeLogService.SavePermissionChangeLogAsync(new PermissionChangeLog
        {
            UserSessionId = UserSession.SessionId,
            UserId = UserSession.CurrentUser?.Id,
            ActionType = actionType,
            RoleId = roleId,
            RoleName = roleName,
            OldPermissions = oldPermissions.OrderBy(p => p.ToString()).Select(p => p.ToString()).ToList(),
            NewPermissions = newPermissions.OrderBy(p => p.ToString()).Select(p => p.ToString()).ToList()
        }).GetAwaiter().GetResult();
    }
}
