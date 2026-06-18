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
            Console.WriteLine("[5] Manage permission types");
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
                case "5": Console.Clear(); ManagePermissions(); break;
                case "0": return;
            }
        }
    }

    private void ViewAllRoles()
    {
        var roles = _roleService.GetAllRoles();
        var allPerms = _roleService.GetAllPermissions()
            .ToDictionary(p => p.Name, p => p.Description);

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

            if (role.Name == "SuperAdmin")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("    • All permissions");
                Console.ResetColor();
            }
            else if (role.Permissions.Count == 0)
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
                    string desc = allPerms.TryGetValue(perm, out var d) ? d : perm;
                    Console.WriteLine($"    • {desc}");
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

        var allPerms = _roleService.GetAllPermissions();
        var permissions = SelectPermissions(new HashSet<string>(), allPerms);

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

        var current = new HashSet<string>(role.Permissions);
        var allPerms = _roleService.GetAllPermissions();

        Console.WriteLine($"\nCurrent permissions for '{role.Name}':");
        if (current.Count == 0)
            Console.WriteLine("  (none)");
        else
            foreach (var p in current)
            {
                var desc = allPerms.FirstOrDefault(x => x.Name == p).Description ?? p;
                Console.WriteLine($"  • {desc}");
            }

        Console.WriteLine();
        var updated = SelectPermissions(current, allPerms);

        var removed = current.Except(updated).ToList();
        var added   = updated.Except(current).ToList();

        if (removed.Count == 0 && added.Count == 0)
        {
            Console.WriteLine("No changes detected.");
            return;
        }

        if (updated.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("A role must have at least one permission. Changes were not saved.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine();
        if (removed.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Permissions to REMOVE:");
            foreach (var p in removed)
            {
                var desc = allPerms.FirstOrDefault(x => x.Name == p).Description ?? p;
                Console.WriteLine($"  - {desc}");
            }
            Console.ResetColor();
        }
        if (added.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Permissions to ADD:");
            foreach (var p in added)
            {
                var desc = allPerms.FirstOrDefault(x => x.Name == p).Description ?? p;
                Console.WriteLine($"  + {desc}");
            }
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

    private void ManagePermissions()
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║    Manage Permission Types     ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("[1] View all permissions");
            Console.WriteLine("[2] Create custom permission");
            Console.WriteLine("[3] Delete custom permission");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[0] Back");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("Select: ");

            switch (Console.ReadLine())
            {
                case "1": Console.Clear(); ViewAllPermissions(); Console.ReadKey(); break;
                case "2": Console.Clear(); CreatePermission(); Console.ReadKey(); break;
                case "3": Console.Clear(); DeletePermission(); Console.ReadKey(); break;
                case "0": return;
            }
        }
    }

    private void ViewAllPermissions()
    {
        var perms = _roleService.GetAllPermissions();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== ALL PERMISSIONS ===\n");
        Console.ResetColor();

        foreach (var (name, desc, isBuiltIn) in perms)
        {
            Console.ForegroundColor = isBuiltIn ? ConsoleColor.Yellow : ConsoleColor.Green;
            Console.Write($"  {name}");
            if (isBuiltIn)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" (built-in)");
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"    {desc}");
            Console.ResetColor();
        }
    }

    private void CreatePermission()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== CREATE CUSTOM PERMISSION ===\n");
        Console.ResetColor();

        Console.Write("Permission name (no spaces, e.g. ManageCoupons) — empty to cancel: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name)) return;

        if (name.Contains(' '))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Permission names cannot contain spaces.");
            Console.ResetColor();
            return;
        }

        if (_roleService.PermissionNameExists(name))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"A permission named '{name}' already exists.");
            Console.ResetColor();
            return;
        }

        Console.Write("Description (shown in menus): ");
        var desc = Console.ReadLine()?.Trim() ?? name;

        Console.Write($"\nCreate permission '{name}'? (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y") return;

        bool ok = _roleService.CreatePermission(name, desc);

        Console.ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(ok ? $"Permission '{name}' created." : "Failed to create permission.");
        Console.ResetColor();
    }

    private void DeletePermission()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== DELETE CUSTOM PERMISSION ===\n");
        Console.ResetColor();

        var perms = _roleService.GetAllPermissions().Where(p => !p.IsBuiltIn).ToList();

        if (perms.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No custom permissions exist.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine("Custom permissions:");
        for (int i = 0; i < perms.Count; i++)
            Console.WriteLine($"  [{i + 1}] {perms[i].Name} — {perms[i].Description}");
        Console.WriteLine("  [0] Cancel");
        Console.Write("\nSelect: ");

        if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0 || choice < 1 || choice > perms.Count)
            return;

        var (permName, permDesc, _) = perms[choice - 1];

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\nWarning: deleting '{permName}' will remove it from all roles that use it.");
        Console.ResetColor();
        Console.Write("Are you sure? (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y") return;

        bool ok = _roleService.DeletePermission(permName);

        Console.ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(ok ? $"Permission '{permName}' deleted." : "Failed to delete permission.");
        Console.ResetColor();
    }

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

    private static HashSet<string> SelectPermissions(
        HashSet<string> current,
        List<(string Name, string Description, bool IsBuiltIn)> allPerms)
    {
        var selected = new HashSet<string>(current);

        while (true)
        {
            Console.WriteLine("\nToggle permissions (enter number to toggle, 0 to confirm):");
            for (int i = 0; i < allPerms.Count; i++)
            {
                bool on = selected.Contains(allPerms[i].Name);
                Console.ForegroundColor = on ? ConsoleColor.Green : ConsoleColor.DarkGray;
                Console.WriteLine($"  [{i + 1}] [{(on ? "X" : " ")}] {allPerms[i].Description}");
                Console.ResetColor();
            }

            Console.Write("\nToggle #: ");
            var input = Console.ReadLine();

            if (input == "0") break;

            if (int.TryParse(input, out int idx) && idx >= 1 && idx <= allPerms.Count)
            {
                var name = allPerms[idx - 1].Name;
                if (!selected.Add(name)) selected.Remove(name);
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
