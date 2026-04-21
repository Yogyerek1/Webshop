using System;

namespace Webshop.api.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders").RequireAuthorization("CustomerOnly");

        group.MapPost("/checkout", () => {});
        group.MapGet("/", () => {});
    }
}
