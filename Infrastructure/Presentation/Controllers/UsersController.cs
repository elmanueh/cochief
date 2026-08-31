using AutoMapper;
using cochief.Infrastructure.Presentation.Dtos;
using Cochief.Domain.Model;
using Cochief.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cochief.Infrastructure.Presentation.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK)]
    public ActionResult<UserResponseDto> GetById(Guid id)
    {
        User user = userService.GetUser(id);

        UserResponseDto response = mapper.Map<UserResponseDto>(user);

        return Ok(response);
    }

    [HttpPatch("{id}/link")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LinkPlayer(Guid id, CreateLinkPlayerRequestDto request)
    {
        _ = await userService.LinkPlayerAsync(id, request.PlayerTag, request.VerificationToken);

        return NoContent();
    }

    [HttpPatch("{id}/unlink")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult UnlinkPlayer(Guid id)
    {
        userService.UnlinkPlayer(id);

        return NoContent();
    }
}
