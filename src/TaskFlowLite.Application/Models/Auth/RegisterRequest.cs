using System.ComponentModel.DataAnnotations;

namespace TaskFlowLite.Application.Models.Auth;

public class RegisterRequest
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
