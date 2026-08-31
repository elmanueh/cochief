using AutoMapper;
using cochief.Infrastructure.Presentation.Dtos;
using Cochief.Domain.Model;
using Cochief.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cochief.Infrastructure.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService, IMapper mapper) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status201Created)]
    public ActionResult<UserResponseDto> Register(CreateUserRequestDto request)
    {
        User user = authService.Register(request.Name, request.Email, request.Password);

        UserResponseDto response = mapper.Map<UserResponseDto>(user);

        return CreatedAtAction(nameof(UsersController.GetById), "Users", new { id = response.Id }, response);
    }

    [HttpPost("login")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK)]
    public ActionResult<UserResponseDto> Login(CreateLoginRequestDto request)
    {
        User user = authService.Login(request.Email, request.Password);

        UserResponseDto response = mapper.Map<UserResponseDto>(user);

        return Ok(response);
    }
}
