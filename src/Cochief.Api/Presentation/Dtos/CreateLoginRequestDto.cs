using System.ComponentModel.DataAnnotations;

namespace Cochief.Api.Presentation.Dtos;

public sealed class CreateLoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
