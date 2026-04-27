using System;
using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;
using Webshop.api.DTOs.ProductDTOs;
using Webshop.api.Models;

namespace Webshop.api.Services;

public class ProductService(AppDbContext db, IHttpContextAccessor httpContextAccessor)
{
    private HttpContext Context => httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");

    public async Task<IResult> GetProducts()
    {
        var userRole = Context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (userRole == "Admin" || userRole == "SuperAdmin")
        {
            return Results.Ok(await db.Products.ToListAsync());
        }

        var products = await db.Products
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                DiscountPercentage = p.DiscountPercentage
            })
            .AsNoTracking()
            .ToListAsync();
        
        return Results.Ok(products);
    }

    public async Task<IResult> GetProduct(int Id)
    {
        var product = await db.Products.FindAsync(Id);
        if (product is null) return Results.NotFound("Product not found.");

        var userRole = Context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;


        if (userRole == "Admin" || userRole == "SuperAdmin")
        {
            return Results.Ok(product);
        }

        var response = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            DiscountPercentage = product.DiscountPercentage
        };

        return Results.Ok(response);
    }

    public async Task<IResult> NewProduct(NewProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            DiscountPercentage = dto.DiscountPercentage,
            PurchasePrice = dto.PurchasePrice
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        return Results.Created($"/products/{product.Id}", product);
    }

    public async Task<IResult> UpdateProduct(UpdateProductDto dto)
    {
        var product = await db.Products.FindAsync(dto.Id);
        if (product is null) return Results.NotFound("Product not found.");

        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.Price != null) product.Price = dto.Price.Value;
        if (dto.StockQuantity != null) product.StockQuantity = dto.StockQuantity.Value;
        if (dto.DiscountPercentage != null) product.DiscountPercentage = dto.DiscountPercentage.Value;
        if (dto.PurchasePrice != null) product.PurchasePrice = dto.PurchasePrice.Value;

        await db.SaveChangesAsync();

        return Results.Ok(new { message = "Product updated successfully", product });
    }

    public async Task<IResult> DeleteProduct(int Id)
    {
        var product = await db.Products.FindAsync(Id);
        if (product is null) return Results.NotFound("Product not found.");

        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return Results.Ok("Product deleted successfully.");
    }
}
