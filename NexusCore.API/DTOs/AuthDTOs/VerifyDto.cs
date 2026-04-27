using System;
using System.ComponentModel.DataAnnotations;

namespace Webshop.api.DTOs;

public class VerifyDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Code { get; set; } = string.Empty;

    public VerificationPurpose Purpose { get; set; }
}
