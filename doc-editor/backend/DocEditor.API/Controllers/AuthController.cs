using DocEditor.API.Data;
using DocEditor.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocEditor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    public AuthController(AppDbContext db) => _db = db;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Username and password are required.");

        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Username == req.Username && u.Password == req.Password);

        if (user is null)
            return Unauthorized("Invalid username or password.");

        return Ok(new LoginResponse(user.Id.ToString(), user.Id, user.Username));
    }
}
