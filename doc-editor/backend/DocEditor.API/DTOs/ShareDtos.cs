namespace DocEditor.API.DTOs;

public record ShareDocumentRequest(int DocumentId, int SharedWithUserId);

public record ShareDto(
    int Id,
    int DocumentId,
    string DocumentTitle,
    int SharedWithUserId,
    string SharedWithUsername,
    DateTime SharedAt
);
