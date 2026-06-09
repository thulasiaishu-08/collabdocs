namespace DocEditor.API.DTOs;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token, int UserId, string Username);
public record UserDto(int Id, string Username, string Email);
