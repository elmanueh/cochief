namespace Cochief.Api.Presentation.Dtos;

public sealed class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<PlayerResponseDto> Players { get; set; } = [];

    public sealed class PlayerResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public int TownHallLevel { get; set; }
        public string? ClanTag { get; set; }
    }
}
