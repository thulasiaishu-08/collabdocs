namespace DocEditor.API.Models;

public class DocumentShare
{
    public int Id { get; set; }
    public DateTime SharedAt { get; set; }

    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public int SharedWithUserId { get; set; }
    public User SharedWithUser { get; set; } = null!;
}
