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
[ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, Description = "A valid JWT access token is required.")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, Description = "An unexpected server error occurred.")]
[Route("api/clans")]
public sealed class ClansController(IClanService clanService, IMapper mapper) : ControllerBase
{
    private readonly IClanService _clanService = clanService;
    private readonly IMapper _mapper = mapper;

    /// <summary>Gets the clans accessible to the authenticated user.</summary>
    [HttpGet(Name = "GetAccessibleClans")]
    [ProducesResponseType<IReadOnlyList<ClanResponseDto>>(StatusCodes.Status200OK, Description = "The accessible clans, or an empty collection.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, Description = "The authenticated user no longer exists.")]
    public async Task<ActionResult<IReadOnlyList<ClanResponseDto>>> Get(CancellationToken cancellationToken)
    {
        IReadOnlyList<Clan> clans = await _clanService.GetByUserIdAsync(User.GetUserId(), cancellationToken);

        IReadOnlyList<ClanResponseDto> response = _mapper.Map<IReadOnlyList<ClanResponseDto>>(clans);

        return Ok(response);
    }

    /// <summary>Gets a clan and its members by clan tag.</summary>
    /// <remarks>The authenticated user's linked player must belong to the requested clan.</remarks>
    /// <param name="tag">Clan tag including the leading hash character.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet("{tag}", Name = "GetClanByTag")]
    [ProducesResponseType<ClanResponseDto>(StatusCodes.Status200OK, Description = "The requested clan and its current members.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, Description = "The clan tag has an invalid format.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, Description = "The user's player does not belong to the requested clan.")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, Description = "The clan or authenticated user was not found.")]
    public async Task<ActionResult<ClanResponseDto>> GetByTag(string tag, CancellationToken cancellationToken)
    {
        Clan clan = await _clanService.GetByTagAsync(User.GetUserId(), tag, cancellationToken);

        ClanResponseDto response = _mapper.Map<ClanResponseDto>(clan);

        return Ok(response);
    }
}
