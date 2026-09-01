using AutoMapper;
using Cochief.Api.Presentation.Dtos;
using Cochief.Domain.Model;

namespace Cochief.Api.Presentation.Mappers;

public sealed class PresentationMappingProfile : Profile
{
    public PresentationMappingProfile()
    {
        CreateMap<User, UserResponseDto>();
    }
}
