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
        app.MapPost("/auth/forgot-password", async (ForgotPasswordDto dto, AuthService authService) => await authService.ForgotPassword(dto));
        app.MapPost("/auth/reset-password", async (ResetPasswordDto dto, AuthService authService) => await authService.ResetPassword(dto));

        app.MapPost("/auth/request-update", async (AuthService authService) => await authService.RequestUpdateCode())
            .RequireAuthorization("CustomerOnly");

        app.MapPut("/auth/update", async (UpdateUserDto dto, AuthService authService) => await authService.Update(dto))
            .RequireAuthorization("CustomerOnly");

        app.MapPost("/auth/me", async (AuthService authService) => await authService.Me())
            .RequireAuthorization("CustomerOnly");

        app.MapGet("/auth/logout", (AuthService authService) => authService.Logout())
            .RequireAuthorization("CustomerOnly");
    }
}
