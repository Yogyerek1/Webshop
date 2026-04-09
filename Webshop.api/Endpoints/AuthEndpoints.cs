using Webshop.api.DTOs;
using Webshop.api.Services;

namespace Webshop.api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (RegisterDto dto, AuthService authService) => await authService.Register(dto));
        app.MapPost("/auth/login", async (LoginDto dto, AuthService authService) => await authService.Login(dto));
        app.MapPost("/auth/verify", async (VerifyDto dto, AuthService authService) => await authService.VerifyCode(dto));
        app.MapGet("/auth/logout", (AuthService authService) => authService.Logout());
    }
}
