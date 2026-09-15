using System.ComponentModel.DataAnnotations;

namespace Cochief.Api.Presentation.Dtos;

/// <summary>Data required to register a Cochief user.</summary>
public sealed class CreateUserRequestDto
{
    /// <example>Chief</example>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <example>chief@example.com</example>
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Password containing at least eight characters.</summary>
    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
