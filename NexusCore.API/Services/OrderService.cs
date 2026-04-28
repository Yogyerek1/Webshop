using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Webshop.api.DTOs.OrderDTOs;
using Webshop.api.Models;

namespace Webshop.api.Services;

public class OrderService(AppDbContext db, HelperService helperService)
{
    public async Task<IResult> Checkout()
    {
        var user = await helperService.GetUserAsync();
        if (user is null) return Results.Unauthorized();

        var cartItems = await db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == user.Id)
            .ToListAsync();
        
        if (cartItems.Count <= 0) return Results.BadRequest("The cart is empty.");

        using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var existingOrder = await db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.UserId == user.Id && o.Status == "Pending");

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in cartItems)
            {
                if (item.Product.StockQuantity < item.Quantity)
                {
                    return Results.BadRequest($"{item.Product.Name} just have {item.Product.StockQuantity} in stock.");
                }

                item.Product.StockQuantity -= item.Quantity;
                
                decimal discountedPrice = (item.Product.Price * (100m - item.Product.DiscountPercentage) / 100m);

                var existingOrderItem = existingOrder?.OrderItems
                    .FirstOrDefault(oi => oi.ProductId == item.ProductId);
                
                if (existingOrder != null) existingOrderItem?.Quantity += item.Quantity;
                else
                {
                    var newItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = discountedPrice
                    };
                    
                    if (existingOrder != null)
                        existingOrder.OrderItems.Add(newItem);
                    else
                        orderItems.Add(newItem);
                }

                totalAmount += (item.Product.Price * (100m - item.Product.DiscountPercentage) / 100m) * item.Quantity;
            }

            if (existingOrder != null)
            {
                existingOrder.OrderItems.AddRange(orderItems);
                existingOrder.TotalAmount += totalAmount;
                existingOrder.OrderDate = DateTime.UtcNow;
            } else
            {
                var order = new Order
                {
                    UserId = user.Id,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    Status = "Pending",
                    OrderItems = orderItems
                };

                db.Orders.Add(order);   
            }

            db.CartItems.RemoveRange(cartItems);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Results.Ok(new
            {
                Message = existingOrder != null ? "Order updated." : "Order placed successfully."
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Results.Problem("Error during checkout: " + ex.Message);
        }
    }

    public async Task<IResult> GetOrder()
    {
        var user = await helperService.GetUserAsync();
        if (user is null) return Results.Unauthorized();

        var order = await db.Orders
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderDto {
                Id = o.Id,
                CustomerName = user.Username,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            })
            .FirstOrDefaultAsync();
        
        if (order is null) return Results.NotFound("Order not found.");

        return Results.Ok(order);
    }

    public async Task<IResult> GetOrders()
    {
        var orders = await db.Orders
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderDto {
                Id = o.Id,
                CustomerName = o.User.Username,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            })
            .ToListAsync();
        
        return Results.Ok(orders);
    }

    public async Task<IResult> DeleteOrder(int Id)
    {
        var user = await helperService.GetUserAsync();
        if (user is null) return Results.Unauthorized();

        var order = await db.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == Id);
        
        if (order is null) return Results.NotFound("Order not found.");

        if (user.Role != "Admin" && user.Role != "SuperAdmin" && order.UserId != user.Id)
        {
            return Results.Forbid();
        }

        using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            foreach (var item in order.OrderItems)
            {
                if (item.Product != null)
                {
                    item.Product.StockQuantity += item.Quantity;
                }
            }

            db.Orders.Remove(order);
            
            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Results.Ok(new { Message = $"Order deleted." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Results.Problem("Error during process: " + ex.Message);
        }
    }
}
