using AutoMapper;
using Cochief.Api.Authentication;
using Cochief.Api.Presentation.Dtos;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cochief.Api.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    [HttpGet("me")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponseDto>> GetMe(CancellationToken cancellationToken)
    {
        User user = await _userService.GetUserAsync(User.GetUserId(), cancellationToken);

        UserResponseDto response = _mapper.Map<UserResponseDto>(user);

        return Ok(response);
    }

    [HttpPatch("me/link")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LinkPlayer(CreateLinkPlayerRequestDto request, CancellationToken cancellationToken)
    {
        await _userService.LinkPlayerAsync(User.GetUserId(), request.PlayerTag, request.VerificationToken, cancellationToken);

        return NoContent();
    }

    [HttpPatch("me/unlink")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnlinkPlayer(CancellationToken cancellationToken)
    {
        await _userService.UnlinkPlayerAsync(User.GetUserId(), cancellationToken);

        return NoContent();
    }
}
