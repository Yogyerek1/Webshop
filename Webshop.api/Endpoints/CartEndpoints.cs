using System;

namespace Webshop.api.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/cart", () => {});
        app.MapPost("/cart/items", () => {});
        app.MapDelete("cart/items/{id}", (int id) => {});
    }
}
