using DocEditor.API.DTOs;
using DocEditor.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocEditor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _svc;
    public DocumentsController(IDocumentService svc) => _svc = svc;

    private int UserId => (int)HttpContext.Items["UserId"]!;

    [HttpGet]
    public async Task<IActionResult> GetMine() =>
        Ok(await _svc.GetMyDocumentsAsync(UserId));

    [HttpGet("shared")]
    public async Task<IActionResult> GetShared() =>
        Ok(await _svc.GetSharedDocumentsAsync(UserId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var doc = await _svc.GetDocumentAsync(id, UserId);
        return doc is null ? NotFound() : Ok(doc);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            return BadRequest("Title is required.");
        if (req.Title.Length > 200)
            return BadRequest("Title must be 200 characters or fewer.");

        var doc = await _svc.CreateDocumentAsync(UserId, req);
        return CreatedAtAction(nameof(Get), new { id = doc.Id }, doc);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentRequest req)
    {
        var doc = await _svc.UpdateDocumentAsync(id, UserId, req);
        return doc is null ? NotFound() : Ok(doc);
    }

    [HttpPatch("{id:int}/rename")]
    public async Task<IActionResult> Rename(int id, [FromBody] RenameDocumentRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            return BadRequest("Title is required.");
        if (req.Title.Length > 200)
            return BadRequest("Title must be 200 characters or fewer.");

        var doc = await _svc.RenameDocumentAsync(id, UserId, req);
        return doc is null ? NotFound() : Ok(doc);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _svc.DeleteDocumentAsync(id, UserId);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("upload")]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".txt" && ext != ".md")
            return BadRequest("Only .txt and .md files are supported.");

        string raw;
        using (var reader = new StreamReader(file.OpenReadStream()))
            raw = await reader.ReadToEndAsync();

        var html = ext == ".md"
            ? $"<pre style=\"font-family:monospace;white-space:pre-wrap\">{System.Net.WebUtility.HtmlEncode(raw)}</pre>"
            : $"<p>{System.Net.WebUtility.HtmlEncode(raw).Replace("\n", "<br>")}</p>";

        var title = Path.GetFileNameWithoutExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(title)) title = "Untitled";

        var doc = await _svc.UploadDocumentAsync(UserId, title, html);
        return CreatedAtAction(nameof(Get), new { id = doc.Id }, doc);
    }

    [HttpGet("{id:int}/shares")]
    public async Task<IActionResult> GetShares(int id) =>
        Ok(await _svc.GetDocumentSharesAsync(id, UserId));
}
