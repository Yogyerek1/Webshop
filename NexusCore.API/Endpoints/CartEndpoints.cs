using System;
using Webshop.api.DTOs.CartItemDTOs;
using Webshop.api.Services;

namespace Webshop.api.Endpoints;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cart").RequireAuthorization("CustomerLevel");

        group.MapGet("/", (CartService cartService) => cartService.GetItems());
        group.MapPost("/items", ([Microsoft.AspNetCore.Mvc.FromBody] NewCartItemDto dto, CartService cartService) => cartService.NewItem(dto));
        group.MapPut("/items", ([Microsoft.AspNetCore.Mvc.FromBody] UpdateCartItem dto, CartService cartService) => cartService.UpdateItem(dto));
        group.MapDelete("/items/{id}", (int Id, CartService cartService) => cartService.DeleteItem(Id));
    }
}
