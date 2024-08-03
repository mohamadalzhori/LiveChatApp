namespace Server.Data.Models;

public class Message
{
    public int Id { get; set; }
    public required string FromUser { get; set; }
    public required string ToUser { get; set; }
    public required string Content { get; set; }
}