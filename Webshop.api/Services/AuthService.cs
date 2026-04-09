using Microsoft.EntityFrameworkCore;
using Webshop.api.DTOs;
using Webshop.api.Models;

namespace Webshop.api.Services;

public class AuthService(AppDbContext db)
{
    public async Task<IResult> Register(RegisterDto dto)
    {
        var exists = await db.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) return Results.Conflict("This email is already taken.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Customer"
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Results.Created($"/auth/profile", new { user.Id, user.Username, user.Email, user.Role });
    }
    public void Login(LoginDto dto) {}
    public void Profile() {}
}
