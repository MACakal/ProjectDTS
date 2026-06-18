using Npgsql;
namespace ProjectDTS;

public class RoleService
{
    private readonly DatabaseService _db;
    private readonly PermissionChangeLogService _permissionChangeLogService;

    public RoleService(DatabaseService db, PermissionChangeLogService permissionChangeLogService)
    {
        _db = db;
        _permissionChangeLogService = permissionChangeLogService;
    }

    public HashSet<string> GetPermissionsForUser(User user)
    {
        if (user.Role == "SuperAdmin")
            return GetAllPermissions().Select(p => p.Name).ToHashSet();
        return GetPermissionsForRoleName(user.Role);
    }

    private HashSet<string> GetPermissionsForRoleName(string roleName)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        const string sql = @"SELECT p.name FROM role_permissions rp
                             JOIN roles r ON r.id = rp.role_id
                             JOIN permissions p ON p.id = rp.permission_id
                             WHERE r.name = @name";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("name", roleName);

        var result = new HashSet<string>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add(reader.GetString(0));
        return result;
    }

    public List<Role> GetAllRoles()
    {
        using var conn = _db.GetConnection();
        conn.Open();

        const string sql = @"SELECT r.id, r.name, r.is_builtin, p.name
                             FROM roles r
                             LEFT JOIN role_permissions rp ON rp.role_id = r.id
                             LEFT JOIN permissions p ON p.id = rp.permission_id
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
            if (!reader.IsDBNull(3))
                role.Permissions.Add(reader.GetString(3));
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

    public bool CreateRole(string name, HashSet<string> permissions)
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

            LogPermissionChange("RoleCreated", roleId, name, new HashSet<string>(), permissions);

            return true;
        }
        catch
        {
            if (!committed) tx.Rollback();
            return false;
        }
    }

    public bool UpdateRolePermissions(int roleId, HashSet<string> permissions)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        var role = GetAllRoles().First(r => r.Id == roleId);
        var oldPermissions = new HashSet<string>(role.Permissions);
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
            if (!committed) tx.Rollback();
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
            LogPermissionChange("RoleDeleted", role.Id, role.Name, new HashSet<string>(role.Permissions), new HashSet<string>());

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

    // --- Permission type management ---

    public List<(string Name, string Description, bool IsBuiltIn)> GetAllPermissions()
    {
        using var conn = _db.GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "SELECT name, description, is_builtin FROM permissions ORDER BY is_builtin DESC, name", conn);
        using var reader = cmd.ExecuteReader();
        var result = new List<(string, string, bool)>();
        while (reader.Read())
            result.Add((reader.GetString(0), reader.GetString(1), reader.GetBoolean(2)));
        return result;
    }

    public bool PermissionNameExists(string name)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT 1 FROM permissions WHERE LOWER(name) = LOWER(@name)", conn);
        cmd.Parameters.AddWithValue("name", name);
        return cmd.ExecuteScalar() != null;
    }

    public bool CreatePermission(string name, string description)
    {
        try
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "INSERT INTO permissions (name, description, is_builtin) VALUES (@name, @desc, false)", conn);
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("desc", description);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch { return false; }
    }

    public bool DeletePermission(string name)
    {
        try
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM permissions WHERE name = @name AND is_builtin = false", conn);
            cmd.Parameters.AddWithValue("name", name);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch { return false; }
    }

    private static void InsertPermissions(NpgsqlConnection conn, NpgsqlTransaction tx, int roleId, HashSet<string> permissions)
    {
        foreach (var perm in permissions)
        {
            using var ins = new NpgsqlCommand(
                @"INSERT INTO role_permissions (role_id, permission_id)
                  SELECT @rid, id FROM permissions WHERE name = @perm
                  ON CONFLICT (role_id, permission_id) DO NOTHING", conn, tx);
            ins.Parameters.AddWithValue("rid", roleId);
            ins.Parameters.AddWithValue("perm", perm);
            ins.ExecuteNonQuery();
        }
    }

    private void LogPermissionChange(
        string actionType,
        int roleId,
        string roleName,
        HashSet<string> oldPermissions,
        HashSet<string> newPermissions)
    {
        _permissionChangeLogService.SavePermissionChangeLogAsync(new PermissionChangeLog
        {
            UserSessionId = UserSession.SessionId,
            UserId = UserSession.CurrentUser?.Id,
            ActionType = actionType,
            RoleId = roleId,
            RoleName = roleName,
            OldPermissions = oldPermissions.OrderBy(p => p).ToList(),
            NewPermissions = newPermissions.OrderBy(p => p).ToList()
        }).GetAwaiter().GetResult();
    }
}
