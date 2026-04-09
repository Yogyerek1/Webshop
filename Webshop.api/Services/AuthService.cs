using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Webshop.api.DTOs;
using Webshop.api.Models;

namespace Webshop.api.Services;

public class AuthService(AppDbContext db, IConfiguration config, IHttpContextAccessor httpContextAccessor)
{
    private HttpContext Context => httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available!");
    public async Task<IResult> Register(RegisterDto dto)
    {
        var exists = await db.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) return Results.Conflict("This email is already taken.");

        var code = new Random().Next(100000, 999999).ToString();

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Customer",
            IsVerified = false,
            VerifyCode = code,
            CodeExpiry = DateTime.UtcNow.AddMinutes(10)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        // email code
        Console.WriteLine($"[REGISTRATION -> ACCOUNT VERIFICATION] Email sent to ({dto.Email}): {code}!");

        return Results.Ok("Registration successfully! Please, check your email to verificate your account!");
    }

    public async Task<IResult> Login(LoginDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return Results.Unauthorized();

        var code = new Random().Next(100000, 999999).ToString();
        user.VerifyCode = code;
        user.CodeExpiry = DateTime.UtcNow.AddMinutes(5);

        await db.SaveChangesAsync();

        Console.WriteLine($"[LOGIN 2FA] Email has been sent: {user.Email} - Code: {code}");
        return Results.Ok("Please, check your email. The verification has been sent.");
    }

    public async Task<IResult> Me()
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
        if (user is null) return Results.Unauthorized();

        return Results.Ok(
            new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role
            }
        );
    }

    public IResult Logout()
    {
        Context.Response.Cookies.Delete("access_token");
        return Results.Ok("Logged out successfully!");
    }

    public async Task<IResult> VerifyCode(VerifyDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null) return Results.NotFound("The user is not found!");
        if (user.VerifyCode != dto.Code) return Results.BadRequest("Wrong code!");
        if (user.CodeExpiry < DateTime.UtcNow) return Results.BadRequest("The code is expired!");

        object responseMessage;

        if (!user.IsVerified)
        {
            // register verification
            user.IsVerified = true;
            responseMessage = new { Message = "Successfully account verification!" };
        }
        else
        {
            // login verification
            responseMessage = new
            {
                Message = "Successfully logged in!",
                User = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Role
                }
            };
        }

        user.VerifyCode = null;
        user.CodeExpiry = null;
        await db.SaveChangesAsync();

        GenerateJwtToken(user);

        return Results.Ok(responseMessage);
    }

    private void GenerateJwtToken(User user)
    {
        var jwtSecret = config["JWT_SECRET"] ?? throw new InvalidOperationException("JWT_SECRET not configured!");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        Context.Response.Cookies.Append("access_token", new JwtSecurityTokenHandler().WriteToken(token), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }
}
