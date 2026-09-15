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
[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, Description = "An unexpected server error occurred.")]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService, IMapper mapper) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IMapper _mapper = mapper;

    /// <summary>Registers a new Cochief user.</summary>
    [HttpPost("register", Name = "RegisterUser")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status201Created, Description = "The user was registered successfully.")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, Description = "The request data is invalid.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, Description = "A user with the supplied email already exists.")]
    public async Task<ActionResult<UserResponseDto>> Register(CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        User user = await _authService.RegisterAsync(request.Name, request.Email, request.Password, cancellationToken);

        UserResponseDto response = _mapper.Map<UserResponseDto>(user);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>Authenticates a user with email and password.</summary>
    [HttpPost("login", Name = "LoginUser")]
    [ProducesResponseType<AuthenticationResponseDto>(StatusCodes.Status200OK, Description = "Authentication succeeded.")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, Description = "The request data is invalid.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, Description = "The credentials are invalid.")]
    public async Task<ActionResult<AuthenticationResponseDto>> Login(CreateLoginRequestDto request, CancellationToken cancellationToken)
    {
        UserAuthentication result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

        AuthenticationResponseDto response = _mapper.Map<AuthenticationResponseDto>(result);

        return Ok(response);
    }

    /// <summary>Renews an authentication session using its refresh token.</summary>
    /// <remarks>The refresh token must be sent as a Bearer token.</remarks>
    [HttpPost("refresh", Name = "RefreshAuthentication")]
    [ProducesResponseType<AuthenticationResponseDto>(StatusCodes.Status200OK, Description = "The session was renewed successfully.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, Description = "The refresh token is missing, invalid or expired.")]
    public async Task<ActionResult<AuthenticationResponseDto>> Refresh(CancellationToken cancellationToken)
    {
        string token = Request.GetBearerToken();
        UserAuthentication result = await _authService.RefreshAsync(token, cancellationToken);

        AuthenticationResponseDto response = _mapper.Map<AuthenticationResponseDto>(result);

        return Ok(response);
    }

    /// <summary>Revokes the current authentication session.</summary>
    /// <remarks>The refresh token must be sent as a Bearer token.</remarks>
    [HttpPost("logout", Name = "LogoutUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Description = "The session was revoked successfully.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, Description = "The refresh token is missing or invalid.")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        string token = Request.GetBearerToken();
        await _authService.LogoutAsync(token, cancellationToken);

        return NoContent();
    }
}
