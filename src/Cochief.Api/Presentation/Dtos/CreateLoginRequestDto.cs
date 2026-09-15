using System.ComponentModel.DataAnnotations;

namespace Cochief.Api.Presentation.Dtos;

/// <summary>Credentials used to authenticate a user.</summary>
public sealed class CreateLoginRequestDto
{
    /// <example>chief@example.com</example>
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>The user's password.</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
