namespace DocEditor.API.DTOs;

public record CreateDocumentRequest(string Title, string? Content);
public record UpdateDocumentRequest(string Content);
public record RenameDocumentRequest(string Title);

public record DocumentSummaryDto(
    int Id,
    string Title,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int OwnerId,
    string OwnerName
);

public record DocumentDto(
    int Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int OwnerId,
    string OwnerName
);
