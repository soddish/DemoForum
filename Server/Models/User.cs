namespace DemoForum.Models;

public class User
{
    public int Id { get; set; }
    public DateTime CreationTime { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime? BannedUntil { get; set; }
}