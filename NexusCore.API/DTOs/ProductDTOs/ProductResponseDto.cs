using System;

namespace Webshop.api.DTOs.ProductDTOs;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public decimal DiscountPercentage { get; set; }
}
