using System;
using System.ComponentModel.DataAnnotations;

namespace Webshop.api.DTOs.CartItemDTOs;

public class NewCartItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
