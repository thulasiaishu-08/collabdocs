using DocEditor.API.DTOs;

namespace DocEditor.API.Services;

public interface IDocumentService
{
    Task<IEnumerable<DocumentSummaryDto>> GetMyDocumentsAsync(int userId);
    Task<IEnumerable<DocumentSummaryDto>> GetSharedDocumentsAsync(int userId);
    Task<DocumentDto?> GetDocumentAsync(int id, int userId);
    Task<DocumentDto> CreateDocumentAsync(int userId, CreateDocumentRequest request);
    Task<DocumentDto?> UpdateDocumentAsync(int id, int userId, UpdateDocumentRequest request);
    Task<DocumentDto?> RenameDocumentAsync(int id, int userId, RenameDocumentRequest request);
    Task<bool> DeleteDocumentAsync(int id, int userId);
    Task<DocumentDto> UploadDocumentAsync(int userId, string title, string content);
    Task<ShareDto?> ShareDocumentAsync(int userId, ShareDocumentRequest request);
    Task<IEnumerable<ShareDto>> GetDocumentSharesAsync(int documentId, int userId);
    Task<bool> RemoveShareAsync(int shareId, int userId);
}
