using DocEditor.API.Data;
using DocEditor.API.DTOs;
using DocEditor.API.Models;
using DocEditor.API.Services;
using Microsoft.EntityFrameworkCore;

namespace DocEditor.Tests;

public class DocumentServiceTests
{
    private static AppDbContext BuildContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var ctx = new AppDbContext(options);
        ctx.Users.AddRange(
            new User { Id = 1, Username = "alice", Password = "password1", Email = "alice@example.com" },
            new User { Id = 2, Username = "bob",   Password = "password2", Email = "bob@example.com"   }
        );
        ctx.SaveChanges();
        return ctx;
    }

    [Fact]
    public async Task CreateDocument_ReturnsCorrectTitleAndOwner()
    {
        await using var ctx = BuildContext();
        var svc = new DocumentService(ctx);

        var doc = await svc.CreateDocumentAsync(1, new CreateDocumentRequest("Hello World", "<p>content</p>"));

        Assert.Equal("Hello World", doc.Title);
        Assert.Equal("<p>content</p>", doc.Content);
        Assert.Equal(1, doc.OwnerId);
        Assert.Equal("alice", doc.OwnerName);
    }

    [Fact]
    public async Task GetMyDocuments_ReturnsOnlyOwnedDocuments()
    {
        await using var ctx = BuildContext();
        var svc = new DocumentService(ctx);

        await svc.CreateDocumentAsync(1, new CreateDocumentRequest("Alice Doc A", ""));
        await svc.CreateDocumentAsync(1, new CreateDocumentRequest("Alice Doc B", ""));
        await svc.CreateDocumentAsync(2, new CreateDocumentRequest("Bob Doc",    ""));

        var results = await svc.GetMyDocumentsAsync(1);

        Assert.Equal(2, results.Count());
        Assert.All(results, d => Assert.Equal(1, d.OwnerId));
    }

    [Fact]
    public async Task RenameDocument_UpdatesTitle()
    {
        await using var ctx = BuildContext();
        var svc = new DocumentService(ctx);
        var doc = await svc.CreateDocumentAsync(1, new CreateDocumentRequest("Old Title", ""));

        var renamed = await svc.RenameDocumentAsync(doc.Id, 1, new RenameDocumentRequest("New Title"));

        Assert.NotNull(renamed);
        Assert.Equal("New Title", renamed!.Title);
    }

    [Fact]
    public async Task ShareDocument_MakesDocumentVisibleInSharedList()
    {
        await using var ctx = BuildContext();
        var svc = new DocumentService(ctx);
        var doc = await svc.CreateDocumentAsync(1, new CreateDocumentRequest("Shared Doc", "<p>hi</p>"));

        var share = await svc.ShareDocumentAsync(1, new ShareDocumentRequest(doc.Id, 2));
        var shared = await svc.GetSharedDocumentsAsync(2);

        Assert.NotNull(share);
        Assert.Single(shared);
        Assert.Equal("Shared Doc", shared.First().Title);
    }

    [Fact]
    public async Task DeleteDocument_RemovesItFromMyDocuments()
    {
        await using var ctx = BuildContext();
        var svc = new DocumentService(ctx);
        var doc = await svc.CreateDocumentAsync(1, new CreateDocumentRequest("To Delete", ""));

        var deleted = await svc.DeleteDocumentAsync(doc.Id, 1);
        var mine    = await svc.GetMyDocumentsAsync(1);

        Assert.True(deleted);
        Assert.Empty(mine);
    }

    [Fact]
    public async Task ShareDocument_CannotShareWithSelf()
    {
        await using var ctx = BuildContext();
        var svc = new DocumentService(ctx);
        var doc = await svc.CreateDocumentAsync(1, new CreateDocumentRequest("Doc", ""));

        var result = await svc.ShareDocumentAsync(1, new ShareDocumentRequest(doc.Id, 1));

        Assert.Null(result);
    }

    [Fact]
    public async Task GetDocument_ReturnsNullForUnauthorizedUser()
    {
        await using var ctx = BuildContext();
        var svc = new DocumentService(ctx);
        var doc = await svc.CreateDocumentAsync(1, new CreateDocumentRequest("Private", ""));

        // Bob has no access
        var result = await svc.GetDocumentAsync(doc.Id, 2);

        Assert.Null(result);
    }
}
