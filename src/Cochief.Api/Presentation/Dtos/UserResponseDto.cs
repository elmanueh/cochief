namespace Cochief.Api.Presentation.Dtos;

/// <summary>A Cochief user and their linked players.</summary>
public sealed class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>Players linked to the user. The current domain supports at most one.</summary>
    public IReadOnlyList<PlayerResponseDto> Players { get; set; } = [];

    /// <summary>A Clash of Clans player linked to the user.</summary>
    public sealed class PlayerResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>Player tag including the leading hash character.</summary>
        /// <example>#2ABC123</example>
        public string Tag { get; set; } = string.Empty;

        /// <summary>Current town hall level.</summary>
        public int TownHallLevel { get; set; }

        /// <summary>Current clan tag, or null when the player does not belong to a clan.</summary>
        /// <example>#9XYZ456</example>
        public string? ClanTag { get; set; }
    }
}
