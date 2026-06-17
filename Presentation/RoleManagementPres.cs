namespace ProjectDTS;

public class RoleManagementPres
{
    private readonly RoleService _roleService;

    public RoleManagementPres(RoleService roleService)
    {
        _roleService = roleService;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════╗");
            Console.WriteLine("║      Manage Roles          ║");
            Console.WriteLine("╚════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("[1] View all roles and permissions");
            Console.WriteLine("[2] Create new role");
            Console.WriteLine("[3] Edit role permissions");
            Console.WriteLine("[4] Delete custom role");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[0] Back");
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Select option: ");
            Console.ResetColor();

            switch (Console.ReadLine())
            {
                case "1": Console.Clear(); ViewAllRoles(); Console.ReadKey(); break;
                case "2": Console.Clear(); CreateRole(); Console.ReadKey(); break;
                case "3": Console.Clear(); EditRolePermissions(); Console.ReadKey(); break;
                case "4": Console.Clear(); DeleteRole(); Console.ReadKey(); break;
                case "0": return;
            }
        }
    }

    private void ViewAllRoles()
    {
        var roles = _roleService.GetAllRoles();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== ALL ROLES ===\n");
        Console.ResetColor();

        foreach (var role in roles)
        {
            Console.ForegroundColor = role.IsBuiltIn ? ConsoleColor.Yellow : ConsoleColor.Green;
            Console.Write($"  {role.Name}");
            if (role.IsBuiltIn)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" (built-in)");
            }
            Console.WriteLine();
            Console.ResetColor();

            if (role.Permissions.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("    (no permissions)");
                Console.ResetColor();
            }
            else
            {
                foreach (var perm in role.Permissions)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"    • {PermissionDescriptions.Describe(perm)}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
        }
    }

    private void CreateRole()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== CREATE NEW ROLE ===\n");
        Console.ResetColor();

        Console.Write("Role name (leave empty to cancel): ");
        var name = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(name)) return;

        if (_roleService.RoleNameExists(name))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"A role named '{name}' already exists.");
            Console.ResetColor();
            return;
        }

        var permissions = SelectPermissions(new HashSet<Permission>());

        Console.WriteLine();
        Console.Write($"Create role '{name}' with {permissions.Count} permission(s)? (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y") return;

        bool ok = _roleService.CreateRole(name, permissions);

        Console.ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(ok ? $"Role '{name}' created successfully." : "Failed to create role.");
        Console.ResetColor();
    }

    private void EditRolePermissions()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== EDIT ROLE PERMISSIONS ===\n");
        Console.ResetColor();

        var role = PickEditableRole();
        if (role == null) return;

        var current = new HashSet<Permission>(role.Permissions);

        Console.WriteLine($"\nCurrent permissions for '{role.Name}':");
        if (current.Count == 0)
            Console.WriteLine("  (none)");
        else
            foreach (var p in current)
                Console.WriteLine($"  • {PermissionDescriptions.Describe(p)}");

        Console.WriteLine();
        var updated = SelectPermissions(current);

        if (updated.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("A role must have at least one permission. Changes were not saved.");
            Console.ResetColor();
            return;
        }

        // Show a diff so the admin knows exactly what will change
        var removed = current.Except(updated).ToList();
        var added   = updated.Except(current).ToList();

        if (removed.Count == 0 && added.Count == 0)
        {
            Console.WriteLine("No changes detected.");
            return;
        }

        Console.WriteLine();
        if (removed.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Permissions to REMOVE:");
            foreach (var p in removed)
                Console.WriteLine($"  - {PermissionDescriptions.Describe(p)}");
            Console.ResetColor();
        }
        if (added.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Permissions to ADD:");
            foreach (var p in added)
                Console.WriteLine($"  + {PermissionDescriptions.Describe(p)}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.Write($"Save these changes to '{role.Name}'? (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Cancelled — no changes saved.");
            return;
        }

        bool ok = _roleService.UpdateRolePermissions(role.Id, updated);

        Console.ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(ok ? "Permissions updated successfully." : "Failed to update permissions.");
        Console.ResetColor();
    }

    private void DeleteRole()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== DELETE CUSTOM ROLE ===\n");
        Console.ResetColor();

        var role = PickDeletableRole();
        if (role == null) return;

        if (_roleService.RoleHasUsers(role.Id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Cannot delete '{role.Name}': there are users assigned to this role.");
            Console.ResetColor();
            return;
        }

        Console.Write($"Are you sure you want to delete role '{role.Name}'? (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y") return;

        bool ok = _roleService.DeleteRole(role.Id);

        Console.ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(ok ? $"Role '{role.Name}' deleted." : "Failed to delete role.");
        Console.ResetColor();
    }

    // All roles except SuperAdmin — used for permission editing (built-in roles are now editable)
    private Role? PickEditableRole()
    {
        var roles = _roleService.GetAllRoles()
            .Where(r => r.Name != "SuperAdmin")
            .ToList();

        if (roles.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No editable roles found.");
            Console.ResetColor();
            return null;
        }

        Console.WriteLine("Choose a role to edit:");
        for (int i = 0; i < roles.Count; i++)
        {
            string tag = roles[i].IsBuiltIn ? " (built-in)" : " (custom)";
            Console.WriteLine($"  [{i + 1}] {roles[i].Name}{tag}");
        }

        Console.WriteLine("  [0] Cancel");
        Console.Write("\nSelect: ");

        if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0 || choice < 1 || choice > roles.Count)
            return null;

        return roles[choice - 1];
    }

    // Only non-built-in custom roles — used for deletion
    private Role? PickDeletableRole()
    {
        var roles = _roleService.GetAllRoles().Where(r => !r.IsBuiltIn).ToList();

        if (roles.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No custom roles exist yet.");
            Console.ResetColor();
            return null;
        }

        Console.WriteLine("Choose a custom role to delete:");
        for (int i = 0; i < roles.Count; i++)
            Console.WriteLine($"  [{i + 1}] {roles[i].Name}");

        Console.WriteLine("  [0] Cancel");
        Console.Write("\nSelect: ");

        if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0 || choice < 1 || choice > roles.Count)
            return null;

        return roles[choice - 1];
    }

    // Interactive permission toggle — returns the final selection
    private static HashSet<Permission> SelectPermissions(HashSet<Permission> current)
    {
        var perms = PermissionDescriptions.All;
        var selected = new HashSet<Permission>(current);

        while (true)
        {
            Console.WriteLine("\nToggle permissions (enter number to toggle, 0 to confirm):");
            for (int i = 0; i < perms.Length; i++)
            {
                bool on = selected.Contains(perms[i]);
                Console.ForegroundColor = on ? ConsoleColor.Green : ConsoleColor.DarkGray;
                Console.WriteLine($"  [{i + 1}] [{(on ? "X" : " ")}] {PermissionDescriptions.Describe(perms[i])}");
                Console.ResetColor();
            }

            Console.Write("\nToggle #: ");
            var input = Console.ReadLine();

            if (input == "0") break;

            if (int.TryParse(input, out int idx) && idx >= 1 && idx <= perms.Length)
            {
                var perm = perms[idx - 1];
                if (!selected.Add(perm)) selected.Remove(perm);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input.");
                Console.ResetColor();
            }
        }

        return selected;
    }
}
