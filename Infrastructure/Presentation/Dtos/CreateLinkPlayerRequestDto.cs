using System.ComponentModel.DataAnnotations;

namespace cochief.Infrastructure.Presentation.Dtos;

public sealed record CreateLinkPlayerRequestDto(
    [Required] string PlayerTag,
    [Required] string VerificationToken);
