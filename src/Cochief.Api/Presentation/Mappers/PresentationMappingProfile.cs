using AutoMapper;
using Cochief.Api.Presentation.Dtos;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;

namespace Cochief.Api.Presentation.Mappers;

public sealed class PresentationMappingProfile : Profile
{
    public PresentationMappingProfile()
    {
        CreateMap<Clan, ClanResponseDto>()
            .ForMember(destination => destination.Tag, options => options.MapFrom(source => source.Tag.Value));
        CreateMap<Member, ClanResponseDto.ClanMemberResponseDto>()
            .ForMember(destination => destination.Name, options => options.MapFrom(source => source.Player.Name))
            .ForMember(destination => destination.Tag, options => options.MapFrom(source => source.Player.Tag.Value))
            .ForMember(destination => destination.TownHallLevel, options => options.MapFrom(source => source.Player.TownHallLevel))
            .ForMember(destination => destination.Role, options => options.MapFrom(source => source.Role.ToString()));
        CreateMap<User, UserResponseDto>();
        CreateMap<UserAuthentication, AuthenticationResponseDto>()
            .ForMember(destination => destination.AccessToken, options => options.MapFrom(source => source.Tokens.AccessToken))
            .ForMember(destination => destination.AccessTokenExpiresAt, options => options.MapFrom(source => source.Tokens.AccessTokenExpiresAt))
            .ForMember(destination => destination.RefreshToken, options => options.MapFrom(source => source.Tokens.RefreshToken))
            .ForMember(destination => destination.RefreshTokenExpiresAt, options => options.MapFrom(source => source.Tokens.RefreshTokenExpiresAt));
    }
}
