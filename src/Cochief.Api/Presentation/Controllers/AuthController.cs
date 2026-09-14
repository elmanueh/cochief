using AutoMapper;
using Cochief.Api.Authentication;
using Cochief.Api.Presentation.Dtos;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cochief.Api.Presentation.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService, IMapper mapper) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IMapper _mapper = mapper;

    [HttpPost("register")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserResponseDto>> Register(CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        User user = await _authService.RegisterAsync(request.Name, request.Email, request.Password, cancellationToken);

        UserResponseDto response = _mapper.Map<UserResponseDto>(user);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthenticationResponseDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthenticationResponseDto>> Login(CreateLoginRequestDto request, CancellationToken cancellationToken)
    {
        UserAuthentication result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

        AuthenticationResponseDto response = _mapper.Map<AuthenticationResponseDto>(result);

        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType<AuthenticationResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationResponseDto>> Refresh(CancellationToken cancellationToken)
    {
        string token = Request.GetBearerToken();
        UserAuthentication result = await _authService.RefreshAsync(token, cancellationToken);

        AuthenticationResponseDto response = _mapper.Map<AuthenticationResponseDto>(result);

        return Ok(response);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        string token = Request.GetBearerToken();
        await _authService.LogoutAsync(token, cancellationToken);

        return NoContent();
    }
}
