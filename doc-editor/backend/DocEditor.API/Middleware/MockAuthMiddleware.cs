namespace DocEditor.API.Middleware;

public class MockAuthMiddleware
{
    private readonly RequestDelegate _next;

    public MockAuthMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext ctx)
    {
        var path = ctx.Request.Path.Value ?? string.Empty;

        if (path.StartsWith("/api/auth", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await _next(ctx);
            return;
        }

        var header = ctx.Request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            ctx.Response.StatusCode = 401;
            await ctx.Response.WriteAsync("Unauthorized");
            return;
        }

        var token = header["Bearer ".Length..].Trim();
        if (int.TryParse(token, out int userId))
        {
            ctx.Items["UserId"] = userId;
            await _next(ctx);
        }
        else
        {
            ctx.Response.StatusCode = 401;
            await ctx.Response.WriteAsync("Invalid token");
        }
    }
}
