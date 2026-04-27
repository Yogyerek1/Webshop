using System;
using Webshop.api.Services;

namespace Webshop.api.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/orders/checkout", (OrderService orderService) => orderService.Checkout()).RequireAuthorization("CustomerLevel");
        app.MapGet("/orders", (OrderService orderService) => orderService.GetOrders()).RequireAuthorization("AdminLevel");
        app.MapGet("/orders/order", (OrderService orderService) => orderService.GetOrder()).RequireAuthorization("CustomerLevel");
        app.MapDelete("/orders", (int Id, OrderService orderService) => orderService.DeleteOrder(Id)).RequireAuthorization("CustomerLevel");
    }
}
