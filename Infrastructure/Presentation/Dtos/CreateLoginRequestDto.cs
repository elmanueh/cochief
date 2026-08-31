using System.ComponentModel.DataAnnotations;

namespace cochief.Infrastructure.Presentation.Dtos;

public sealed record CreateLoginRequestDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);
