using AutoMapper;
using Cochief.Api.Presentation.Dtos;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Cochief.Api.Presentation.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    [HttpGet("{id}")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        User user = await _userService.GetUserAsync(id, cancellationToken);

        UserResponseDto response = _mapper.Map<UserResponseDto>(user);

        return Ok(response);
    }

    [HttpPatch("{id}/link")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LinkPlayer(Guid id, CreateLinkPlayerRequestDto request, CancellationToken cancellationToken)
    {
        await _userService.LinkPlayerAsync(id, request.PlayerTag, request.VerificationToken, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id}/unlink")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnlinkPlayer(Guid id, CancellationToken cancellationToken)
    {
        await _userService.UnlinkPlayerAsync(id, cancellationToken);

        return NoContent();
    }
}
