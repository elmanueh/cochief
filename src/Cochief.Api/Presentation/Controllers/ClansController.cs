namespace Cochief.Api.Presentation.Controllers;

using AutoMapper;
using Cochief.Api.Authentication;
using Cochief.Api.Presentation.Dtos;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/clans")]
public sealed class ClansController(IClanService clanService, IMapper mapper) : ControllerBase
{
    private readonly IClanService _clanService = clanService;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ClanResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<ClanResponseDto>>> Get(CancellationToken cancellationToken)
    {
        IReadOnlyList<Clan> clans = await _clanService.GetByUserIdAsync(User.GetUserId(), cancellationToken);

        IReadOnlyList<ClanResponseDto> response = _mapper.Map<IReadOnlyList<ClanResponseDto>>(clans);

        return Ok(response);
    }

    [HttpGet("{tag}")]
    [ProducesResponseType<ClanResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClanResponseDto>> GetByTag(string tag, CancellationToken cancellationToken)
    {
        Clan clan = await _clanService.GetByTagAsync(User.GetUserId(), tag, cancellationToken);

        ClanResponseDto response = _mapper.Map<ClanResponseDto>(clan);

        return Ok(response);
    }
}
