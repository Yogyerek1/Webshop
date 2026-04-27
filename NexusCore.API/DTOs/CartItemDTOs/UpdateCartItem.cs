using System;
using System.ComponentModel.DataAnnotations;

namespace Webshop.api.DTOs.CartItemDTOs;

public class UpdateCartItem
{
    [Required]
    public int CartItemId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
