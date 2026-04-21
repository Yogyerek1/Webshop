using System;

namespace Webshop.api.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cart").RequireAuthorization("CustomerOnly");

        group.MapGet("/", () => {});
        group.MapPost("/items", () => {});
        group.MapDelete("/items/{id}", (int id) => {});
    }
}
