namespace DemoForum.Models;

public class Thread
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SubforumId { get; set; }
    public DateTime CreationTime { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}