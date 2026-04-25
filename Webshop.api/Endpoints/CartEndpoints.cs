using System;
using Webshop.api.Services;

namespace Webshop.api.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cart").RequireAuthorization("CustomerLevel");

        group.MapGet("/", (CartService cartService) => cartService.GetItems());
        group.MapPost("/items", (CartService cartService) => cartService.NewItem());
        group.MapDelete("/items/{id}", (int id, CartService cartService) => cartService.DeleteItem());
    }
}
