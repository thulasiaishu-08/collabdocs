namespace DocEditor.API.Models;

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
}
