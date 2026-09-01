using System.ComponentModel.DataAnnotations;

namespace Cochief.Api.Presentation.Dtos;

public sealed class CreateLinkPlayerRequestDto
{
    [Required]
    public string PlayerTag { get; set; } = string.Empty;

    [Required]
    public string VerificationToken { get; set; } = string.Empty;
}
