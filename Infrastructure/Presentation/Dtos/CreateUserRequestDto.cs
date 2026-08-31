using System.ComponentModel.DataAnnotations;

namespace cochief.Infrastructure.Presentation.Dtos;

public sealed record CreateUserRequestDto(
    [Required] string Name,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password);
