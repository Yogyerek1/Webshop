using System;
using Webshop.api.Models;

namespace Webshop.api.Services;

public class HelperService(IHttpContextAccessor httpContextAccessor, AppDbContext db)
{
    private HttpContext Context => httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");

    public async Task<User?> GetUserAsync()
    {
        var userId = Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
        if (userId is null) return null;

        return await db.Users.FindAsync(int.Parse(userId));
    }
}
