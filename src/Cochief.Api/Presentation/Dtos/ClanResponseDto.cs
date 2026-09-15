namespace Cochief.Api.Presentation.Dtos;

/// <summary>A Clash of Clans clan and its current members.</summary>
public sealed class ClanResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Clan tag including the leading hash character.</summary>
    /// <example>#9XYZ456</example>
    public string Tag { get; set; } = string.Empty;
    public IReadOnlyList<ClanMemberResponseDto> Members { get; set; } = [];

    /// <summary>A player's membership within the clan.</summary>
    public sealed class ClanMemberResponseDto
    {
        public Guid PlayerId { get; set; }

        /// <summary>Player role within the clan.</summary>
        /// <example>Leader</example>
        public string Role { get; set; } = string.Empty;
    }
}
