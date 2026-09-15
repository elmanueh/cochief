using System.ComponentModel.DataAnnotations;

namespace Cochief.Api.Presentation.Dtos;

/// <summary>Data required to verify and link a Clash of Clans player.</summary>
public sealed class CreateLinkPlayerRequestDto
{
    /// <summary>Player tag including the leading hash character.</summary>
    /// <example>#2ABC123</example>
    [Required]
    public string PlayerTag { get; set; } = string.Empty;

    /// <summary>Verification token obtained from the Clash of Clans game settings.</summary>
    [Required]
    public string VerificationToken { get; set; } = string.Empty;
}
