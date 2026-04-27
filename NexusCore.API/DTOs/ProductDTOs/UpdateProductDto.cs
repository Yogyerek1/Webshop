using System;
using System.ComponentModel.DataAnnotations;

namespace Webshop.api.DTOs.ProductDTOs;

public class UpdateProductDto
{
    [Required]
    public int Id { get; set; }

    [MinLength(6)]
    public string? Name { get; set; }

    [MinLength(35)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Price { get; set; }

    [Range(0, int.MaxValue)]
    public int? StockQuantity { get; set; }

    [Range(0, 100)]
    public decimal? DiscountPercentage { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? PurchasePrice { get; set; }
}
