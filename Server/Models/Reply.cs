namespace DemoForum.Models;

public class Reply
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreationTime { get; set; }
    public int ParentThreadId { get; set; }
    public string Content { get; set; } = string.Empty;
}