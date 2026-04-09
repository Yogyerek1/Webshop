using Webshop.api.DTOs;
using Webshop.api.Services;

namespace Webshop.api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", (RegisterDto dto, AuthService authService) => authService.Register(dto));
        app.MapPost("/auth/login", (LoginDto dto, AuthService authService) => authService.Login(dto));
        app.MapGet("/auth/profile", (AuthService authService) => authService.Profile());
    }
}
