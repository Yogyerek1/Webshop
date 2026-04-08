using System;
using System.Runtime.CompilerServices;
using Webshop.api.Services;

namespace Webshop.api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", (AuthService authService) => authService.Register());
        app.MapPost("/auth/login", (AuthService authService) => authService.Login());
        app.MapGet("/auth/profile", (AuthService authService) => authService.Profile());
    }
}
