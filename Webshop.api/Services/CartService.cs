using System;
using Microsoft.EntityFrameworkCore;
using Webshop.api.DTOs.CartItemDTOs;
using Webshop.api.Models;

namespace Webshop.api.Services;

public class CartService(AppDbContext db, HelperService helperService)
{
    public async Task<IResult> GetItems()
    {
        var user = await helperService.GetUserAsync();
        if (user is null) return Results.Unauthorized();

        var cartItems = await db.CartItems
            .Where(c => c.UserId == user.Id)
            .Select(c => new CartItemDto(
                c.Id,
                c.ProductId,
                c.Product.Name,
                c.Product.Price,
                c.Quantity,
                c.Product.Price * c.Quantity
            ))
            .ToListAsync();
        
        return Results.Ok(cartItems);
    }
    
    public async Task<IResult> NewItem(NewCartItemDto dto)
    {
        var user = await helperService.GetUserAsync();
        if (user is null) return Results.Unauthorized();

        var product = await db.Products.FindAsync(dto.ProductId);
        if (product is null) return Results.NotFound("Product not found.");

        if (product.StockQuantity < dto.Quantity) return Results.BadRequest($"Only {product.StockQuantity} items in stock.");

        var existingItem = await db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ProductId == dto.ProductId);
        
        if (existingItem != null) return Results.BadRequest($"{existingItem.Product.Name} is already exists in your cart.");

        var cartItem = new CartItem
        {
            UserId = user.Id,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
        };
        db.CartItems.Add(cartItem);

        await db.SaveChangesAsync();
        
        return Results.Ok("Item added to cart.");
    }

    public async Task<IResult> UpdateItem(UpdateCartItem dto)
    {
        var user = await helperService.GetUserAsync();
        if (user is null) return Results.Unauthorized();

        var existingItem = await db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == user.Id && c.Id== dto.CartItemId);
        
        if (existingItem is null) return Results.NotFound("Item not found in cart.");

        var product = await db.Products.FindAsync(existingItem.ProductId);
        if (product is null) return Results.NotFound("Product not found.");

        if (product.StockQuantity < dto.Quantity) return Results.BadRequest($"Only {product.StockQuantity} items in stock.");

        if (dto.Quantity <= 0)
        {
            db.CartItems.Remove(existingItem);
            await db.SaveChangesAsync();
            return Results.Ok("Item removed from cart because quantity was 0.");
        }

        existingItem.Quantity = dto.Quantity;

        await db.SaveChangesAsync();

        return Results.Ok("Cart updated.");
        
    }

    public async Task<IResult> DeleteItem(int Id)
    {
        var user = await helperService.GetUserAsync();
        if (user is null) return Results.Unauthorized();

        var cartItem = await db.CartItems.FirstOrDefaultAsync(c => c.Id == Id && c.UserId == user.Id);
        if (cartItem is null) return Results.NotFound("Product not found in your cart.");

        db.CartItems.Remove(cartItem);
        await db.SaveChangesAsync();

        return Results.Ok("Product removed from your cart.");
    }
}
