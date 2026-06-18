namespace ProjectDTS;

public static class UserSession
{
    public static User? CurrentUser { get; set; }
    public static string SessionId { get; set; }
    public static bool IsLoggedIn => CurrentUser != null;
    public static HashSet<string> Permissions { get; set; } = new();

    public static bool Can(string permission) => Permissions.Contains(permission);

    public static void Clear()
    {
        CurrentUser = null;
        Permissions = new();
    }
}