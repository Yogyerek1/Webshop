using System;
using System.ComponentModel.DataAnnotations;

namespace Webshop.api.DTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
