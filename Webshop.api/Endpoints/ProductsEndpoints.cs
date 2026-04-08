using System;

namespace Webshop.api.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/products", () => {});
        app.MapGet("/products/{id}", (int id) => {});
        app.MapPost("/products", () => {});
        app.MapPut("/products/{id}", (int id) => {});
        app.MapDelete("/products/{id}", (int id) => {});
    }
}
