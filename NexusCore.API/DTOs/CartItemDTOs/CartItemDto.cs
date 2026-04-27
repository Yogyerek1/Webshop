using System;

namespace Webshop.api.DTOs.CartItemDTOs;

public record CartItemDto(int Id, int ProductId, string ProductName, decimal Price, int Quantity, decimal TotalLinePrice);
