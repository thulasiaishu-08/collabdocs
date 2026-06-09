using DocEditor.API.DTOs;
using DocEditor.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocEditor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SharesController : ControllerBase
{
    private readonly IDocumentService _svc;
    public SharesController(IDocumentService svc) => _svc = svc;

    private int UserId => (int)HttpContext.Items["UserId"]!;

    [HttpPost]
    public async Task<IActionResult> Share([FromBody] ShareDocumentRequest req)
    {
        if (req.DocumentId <= 0 || req.SharedWithUserId <= 0)
            return BadRequest("Invalid document or user ID.");

        var share = await _svc.ShareDocumentAsync(UserId, req);
        if (share is null)
            return BadRequest("Cannot share: document not found, user not found, already shared, or you cannot share with yourself.");

        return Ok(share);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Unshare(int id)
    {
        var ok = await _svc.RemoveShareAsync(id, UserId);
        return ok ? NoContent() : NotFound();
    }
}
