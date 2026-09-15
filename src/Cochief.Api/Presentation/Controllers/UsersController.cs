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
[ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, Description = "A valid JWT access token is required.")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, Description = "An unexpected server error occurred.")]
[Route("api/users")]
public sealed class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    /// <summary>Gets the authenticated user and their linked players.</summary>
    [HttpGet("me", Name = "GetCurrentUser")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK, Description = "The authenticated user.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, Description = "The authenticated user no longer exists.")]
    public async Task<ActionResult<UserResponseDto>> GetMe(CancellationToken cancellationToken)
    {
        User user = await _userService.GetUserAsync(User.GetUserId(), cancellationToken);

        UserResponseDto response = _mapper.Map<UserResponseDto>(user);

        return Ok(response);
    }

    /// <summary>Links a verified Clash of Clans player to the authenticated user.</summary>
    [HttpPatch("me/link", Name = "LinkPlayer")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Description = "The player was linked successfully.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, Description = "The player tag or verification token is invalid.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Description = "The user already has a linked player.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status502BadGateway, Description = "Clash of Clans could not be reached.")]
    public async Task<IActionResult> LinkPlayer(CreateLinkPlayerRequestDto request, CancellationToken cancellationToken)
    {
        await _userService.LinkPlayerAsync(User.GetUserId(), request.PlayerTag, request.VerificationToken, cancellationToken);

        return NoContent();
    }

    /// <summary>Unlinks the player associated with the authenticated user.</summary>
    [HttpPatch("me/unlink", Name = "UnlinkPlayer")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Description = "The player was unlinked successfully.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Description = "The user does not have a linked player.")]
    public async Task<IActionResult> UnlinkPlayer(CancellationToken cancellationToken)
    {
        await _userService.UnlinkPlayerAsync(User.GetUserId(), cancellationToken);

        return NoContent();
    }
}
