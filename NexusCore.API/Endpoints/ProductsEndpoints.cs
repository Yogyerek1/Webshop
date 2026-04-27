using System;
using Webshop.api.DTOs.ProductDTOs;
using Webshop.api.Services;

namespace Webshop.api.Endpoints;

public static class ProductsEndpoints
{
    public static void MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/products", (ProductService productService) => productService.GetProducts());
        app.MapGet("/products/{id}", (int id, ProductService productService) => productService.GetProduct(id));

        app.MapPost("/products", (NewProductDto dto, ProductService productService) => productService.NewProduct(dto))
            .RequireAuthorization("AdminLevel");

        app.MapPut("/products/{id}", (UpdateProductDto dto, ProductService productService) => productService.UpdateProduct(dto))
            .RequireAuthorization("AdminLevel");

        app.MapDelete("/products/{id}", (int id, ProductService productService) => productService.DeleteProduct(id))
            .RequireAuthorization("AdminLevel");
    }
}
