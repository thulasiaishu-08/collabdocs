namespace DocEditor.API.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<DocumentShare> SharedWithMe { get; set; } = new List<DocumentShare>();
}
