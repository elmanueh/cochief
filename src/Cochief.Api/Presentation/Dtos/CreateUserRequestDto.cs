using System.ComponentModel.DataAnnotations;

namespace Cochief.Api.Presentation.Dtos;

public sealed class CreateUserRequestDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
