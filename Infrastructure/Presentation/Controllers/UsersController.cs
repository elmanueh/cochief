using AutoMapper;
using cochief.Infrastructure.Presentation.Dtos;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Cochief.Infrastructure.Presentation.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        User user = await userService.GetUserAsync(id, cancellationToken);

        UserResponseDto response = mapper.Map<UserResponseDto>(user);

        return Ok(response);
    }

    [HttpPatch("{id}/link")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LinkPlayer(Guid id, CreateLinkPlayerRequestDto request, CancellationToken cancellationToken)
    {
        await userService.LinkPlayerAsync(id, request.PlayerTag, request.VerificationToken, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id}/unlink")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnlinkPlayer(Guid id, CancellationToken cancellationToken)
    {
        await userService.UnlinkPlayerAsync(id, cancellationToken);

        return NoContent();
    }
}
