using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webshop.api.Models;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Required]
    [RegularExpression("^(Pending|Processing|Shipped|Delivered|Cancelled)$")]
    public string Status { get; set; } = "Pending";

    public User User { get; set; } = null!;
    public List<OrderItem> OrderItems { get; set; } = [];

}
