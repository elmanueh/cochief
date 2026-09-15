namespace Cochief.Api.Presentation.Dtos;

public sealed class ClanResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public IReadOnlyList<ClanMemberResponseDto> Members { get; set; } = [];

    public sealed class ClanMemberResponseDto
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public int TownHallLevel { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
