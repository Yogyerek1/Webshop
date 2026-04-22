using System;
using System.ComponentModel.DataAnnotations;

namespace Webshop.api.DTOs;

public class UpdateUserDto
{
    [MinLength(6)]
    public string? Username { get; set; } = null;

    [EmailAddress]
    public string? Email { get; set; } = null;

    [MinLength(6)]
    public string? Password { get; set; } = null;

    [MinLength(6)]
    public string? Code { get; set; } = string.Empty;
}