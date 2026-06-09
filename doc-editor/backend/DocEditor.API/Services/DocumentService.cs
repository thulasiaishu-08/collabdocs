using DocEditor.API.Data;
using DocEditor.API.DTOs;
using DocEditor.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DocEditor.API.Services;

public class DocumentService : IDocumentService
{
    private readonly AppDbContext _db;

    public DocumentService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<DocumentSummaryDto>> GetMyDocumentsAsync(int userId) =>
        await _db.Documents
            .Where(d => d.OwnerId == userId)
            .OrderByDescending(d => d.UpdatedAt)
            .Select(d => new DocumentSummaryDto(d.Id, d.Title, d.CreatedAt, d.UpdatedAt, d.OwnerId, d.Owner.Username))
            .ToListAsync();

    public async Task<IEnumerable<DocumentSummaryDto>> GetSharedDocumentsAsync(int userId) =>
        await _db.DocumentShares
            .Where(s => s.SharedWithUserId == userId)
            .OrderByDescending(s => s.SharedAt)
            .Select(s => new DocumentSummaryDto(
                s.Document.Id, s.Document.Title,
                s.Document.CreatedAt, s.Document.UpdatedAt,
                s.Document.OwnerId, s.Document.Owner.Username))
            .ToListAsync();

    public async Task<DocumentDto?> GetDocumentAsync(int id, int userId)
    {
        var doc = await _db.Documents.Include(d => d.Owner).FirstOrDefaultAsync(d => d.Id == id);
        if (doc is null) return null;

        bool isOwner = doc.OwnerId == userId;
        bool isShared = await _db.DocumentShares.AnyAsync(s => s.DocumentId == id && s.SharedWithUserId == userId);
        if (!isOwner && !isShared) return null;

        return ToDto(doc);
    }

    public async Task<DocumentDto> CreateDocumentAsync(int userId, CreateDocumentRequest request)
    {
        var doc = new Document
        {
            Title = request.Title.Trim(),
            Content = request.Content ?? string.Empty,
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.Documents.Add(doc);
        await _db.SaveChangesAsync();

        await _db.Entry(doc).Reference(d => d.Owner).LoadAsync();
        return ToDto(doc);
    }

    public async Task<DocumentDto?> UpdateDocumentAsync(int id, int userId, UpdateDocumentRequest request)
    {
        var doc = await _db.Documents.Include(d => d.Owner).FirstOrDefaultAsync(d => d.Id == id);
        if (doc is null) return null;

        bool isOwner = doc.OwnerId == userId;
        bool isShared = await _db.DocumentShares.AnyAsync(s => s.DocumentId == id && s.SharedWithUserId == userId);
        if (!isOwner && !isShared) return null;

        doc.Content = request.Content;
        doc.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ToDto(doc);
    }

    public async Task<DocumentDto?> RenameDocumentAsync(int id, int userId, RenameDocumentRequest request)
    {
        var doc = await _db.Documents.Include(d => d.Owner)
            .FirstOrDefaultAsync(d => d.Id == id && d.OwnerId == userId);
        if (doc is null) return null;

        doc.Title = request.Title.Trim();
        doc.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ToDto(doc);
    }

    public async Task<bool> DeleteDocumentAsync(int id, int userId)
    {
        var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == id && d.OwnerId == userId);
        if (doc is null) return false;

        _db.Documents.Remove(doc);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<DocumentDto> UploadDocumentAsync(int userId, string title, string content) =>
        await CreateDocumentAsync(userId, new CreateDocumentRequest(title, content));

    public async Task<ShareDto?> ShareDocumentAsync(int userId, ShareDocumentRequest request)
    {
        var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == request.DocumentId && d.OwnerId == userId);
        if (doc is null) return null;
        if (request.SharedWithUserId == userId) return null;

        var target = await _db.Users.FindAsync(request.SharedWithUserId);
        if (target is null) return null;

        bool alreadyShared = await _db.DocumentShares.AnyAsync(
            s => s.DocumentId == request.DocumentId && s.SharedWithUserId == request.SharedWithUserId);
        if (alreadyShared) return null;

        var share = new DocumentShare
        {
            DocumentId = request.DocumentId,
            SharedWithUserId = request.SharedWithUserId,
            SharedAt = DateTime.UtcNow
        };
        _db.DocumentShares.Add(share);
        await _db.SaveChangesAsync();

        return new ShareDto(share.Id, share.DocumentId, doc.Title, target.Id, target.Username, share.SharedAt);
    }

    public async Task<IEnumerable<ShareDto>> GetDocumentSharesAsync(int documentId, int userId)
    {
        var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == documentId && d.OwnerId == userId);
        if (doc is null) return [];

        return await _db.DocumentShares
            .Where(s => s.DocumentId == documentId)
            .Select(s => new ShareDto(
                s.Id, s.DocumentId, s.Document.Title,
                s.SharedWithUserId, s.SharedWithUser.Username, s.SharedAt))
            .ToListAsync();
    }

    public async Task<bool> RemoveShareAsync(int shareId, int userId)
    {
        var share = await _db.DocumentShares.Include(s => s.Document)
            .FirstOrDefaultAsync(s => s.Id == shareId && s.Document.OwnerId == userId);
        if (share is null) return false;

        _db.DocumentShares.Remove(share);
        await _db.SaveChangesAsync();
        return true;
    }

    private static DocumentDto ToDto(Document doc) =>
        new(doc.Id, doc.Title, doc.Content, doc.CreatedAt, doc.UpdatedAt, doc.OwnerId, doc.Owner.Username);
}
