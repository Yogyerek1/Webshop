using System;
using System.ComponentModel.DataAnnotations;

namespace Webshop.api.DTOs;

public class ChangeRoleDto
{
    [Required]
    [EmailAddress]
    public string TargetUserEmail { get; set; } = string.Empty;

    [Required]
    public string NewRole { get; set; } = string.Empty;

    public string? Code { get; set; }
}
