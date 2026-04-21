using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.IdentityModel.Tokens;
using Webshop.api.DTOs;
using Webshop.api.Models;

namespace Webshop.api.Services;

public class AuthService(AppDbContext db, IConfiguration config, IHttpContextAccessor httpContextAccessor, MailService mailService)
{
    private HttpContext Context => httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");

    private async Task<string> SendCode(User user, int minutes)
    {
        var code = new Random().Next(100000, 999999).ToString();
        user.VerifyCode = code;
        user.CodeExpiry = DateTime.UtcNow.AddMinutes(minutes);

        await mailService.SendMailAsync(user.Email, "Verification Code", $"Your verification code is: <b>{code}</b>!");

        return code;
    }
    public async Task<IResult> Register(RegisterDto dto)
    {
        var exists = await db.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) return Results.Conflict("This email is already taken.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Customer",
            IsVerified = false,
        };

        db.Users.Add(user);
        string code = await SendCode(user, 10);

        await db.SaveChangesAsync();

        // email code
        Console.WriteLine($"[REGISTRATION -> ACCOUNT VERIFICATION] Email sent to ({dto.Email}): {code}.");

        return Results.Ok("Registration successful! Please check your email to verify your account!");
    }

    public async Task<IResult> Login(LoginDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return Results.Unauthorized();

        string code = await SendCode(user, 5);

        await db.SaveChangesAsync();

        Console.WriteLine($"[LOGIN -> Verification] Email has been sent to ({user.Email}): {code}.");
        return Results.Ok("Please, check your email. The verification code has been sent.");
    }

    public async Task<IResult> Me()
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
        if (user is null) return Results.NotFound("User not found.");

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

    public async Task<IResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null) return Results.Ok("If this email exists, a reset code has been sent.");

        string code = await SendCode(user, 10);

        await db.SaveChangesAsync();

        Console.WriteLine($"[PASSWORD RESET -> VERIFICATION] Email sent to ({user.Email}): {code}.");

        return Results.Ok("If this email exists, a reset code has been sent.");
    }

    public async Task<IResult> ResetPassword(ResetPasswordDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null) return Results.BadRequest("Invalid request.");
        if (user.VerifyCode != dto.Code) return Results.BadRequest("Wrong code.");
        if (user.CodeExpiry < DateTime.UtcNow) return Results.BadRequest("The code is expired.");

        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        await FinalizeVerification(user);
        return Results.Ok("Password has been reset successfully. You can now log in.");
    }

    public async Task<IResult> RequestUpdateCode()
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
        if (user is null) return Results.NotFound("User not found.");

        string code = await SendCode(user, 5);

        await db.SaveChangesAsync();

        Console.WriteLine($"[UPDATE PROFILE -> VERIFICATION] Code sent to ({user.Email}): {code}.");

        return Results.Ok("Verification code sent to your email.");
    }

    public async Task<IResult> Update(UpdateUserDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value));
        if (user is null) return Results.NotFound("User not found.");

        if (user.VerifyCode != dto.Code || user.CodeExpiry < DateTime.UtcNow) return Results.BadRequest("Invalid or expired verification code.");

        if (!string.IsNullOrWhiteSpace(dto.Username)) user.Username = dto.Username;

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var emailExists = await db.Users.AnyAsync(u => u.Email == dto.Email && u.Id != user.Id);
            if (emailExists) return Results.Conflict("Email is already taken by another user.");
            user.Email = dto.Email;
        }

        if (!string.IsNullOrWhiteSpace(dto.Password)) user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await FinalizeVerification(user);
        return Results.Ok("Profile updated.");
    }

    public async Task<IResult> ChangeUserRole(ChangeRoleDto dto)
    {
        var currentUserId = Context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var adminUser = await db.Users.FindAsync(int.Parse(currentUserId!));

        if (adminUser is null || adminUser.Role != "SuperAdmin")
            return Results.Json(new { message = "Forbidden: Only SuperAdmins can change roles." }, statusCode: 403);
        
        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            string code = await SendCode(adminUser, 5);
            await db.SaveChangesAsync();
            Console.WriteLine($"[ROLE CHANGE -> 2FA] Code sent to SuperAdmin ({adminUser.Email}): {code}");
            return Results.Ok("Verification code sent to your email. Please provide the code to confirm the role change.");
        }

        if (adminUser.VerifyCode != dto.Code || adminUser.CodeExpiry < DateTime.UtcNow)
            return Results.BadRequest("Invalid or expired 2FA code.");

        var targetUser = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.TargetUserEmail);
        if (targetUser is null) return Results.NotFound("Target user not found.");

        if (dto.NewRole != "Admin" && dto.NewRole != "Customer" && dto.NewRole != "SuperAdmin")
            return Results.BadRequest("Invalid role name.");


        if (!targetUser.IsVerified) return Results.BadRequest("The target user is not verified.");
         
        targetUser.Role = dto.NewRole;

        await FinalizeVerification(adminUser);

        return Results.Ok($"User {targetUser.Email} role updated to {dto.NewRole} succesfully.");
    }

    public IResult Logout()
    {
        Context.Response.Cookies.Delete("access_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
        return Results.Ok("Logged out successfully.");
    }

    public async Task<IResult> VerifyCode(VerifyDto dto)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || user.VerifyCode != dto.Code) return Results.BadRequest("Invalid verification code.");
        if (user.CodeExpiry < DateTime.UtcNow) return Results.BadRequest("The code has expired.");

        switch (dto.Purpose)
        {
            case VerificationPurpose.Register:
                if (user.IsVerified) return Results.BadRequest("This account is already verified.");
                user.IsVerified = true;
                await FinalizeVerification(user);
                return Results.Ok("Account verified successful! You can now log in.");
            
            case VerificationPurpose.Login:
                if (!user.IsVerified) return Results.BadRequest("Account not verified yet.");
                await FinalizeVerification(user);
                GenerateJwtToken(user);
                return Results.Ok(new { message = "Logged in successfully!", user.Username });

            default:
                return Results.BadRequest("Invalid verification purpose.");
        }
    }

    private async Task FinalizeVerification(User user)
    {
        user.VerifyCode = null;
        user.CodeExpiry = null;
        await db.SaveChangesAsync();
    }

    private void GenerateJwtToken(User user)
    {
        var jwtSecret = config["JWT_SECRET"] ?? throw new InvalidOperationException("JWT_SECRET not configured.");
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
