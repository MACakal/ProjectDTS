namespace ProjectDTS;

public static class UserSession
{
    public static User? CurrentUser { get; set; }
    public static string SessionId { get; set; }
    public static bool IsLoggedIn => CurrentUser != null;
}