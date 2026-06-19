namespace ProjectDTS;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsBuiltIn { get; set; }
    public List<string> Permissions { get; set; } = new();
}
