using AutoMapper;
using Cochief.Api.Presentation.Dtos;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Cochief.Api.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService, IMapper mapper) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserResponseDto>> Register(CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        User user = await authService.RegisterAsync(request.Name, request.Email, request.Password, cancellationToken);

        UserResponseDto response = mapper.Map<UserResponseDto>(user);

        return CreatedAtAction(nameof(UsersController.GetById), "Users", new { id = response.Id }, response);
    }

    [HttpPost("login")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponseDto>> Login(CreateLoginRequestDto request, CancellationToken cancellationToken)
    {
        User user = await authService.LoginAsync(request.Email, request.Password, cancellationToken);

        UserResponseDto response = mapper.Map<UserResponseDto>(user);

        return Ok(response);
    }
}
