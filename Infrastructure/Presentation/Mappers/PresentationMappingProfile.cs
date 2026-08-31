using AutoMapper;
using cochief.Infrastructure.Presentation.Dtos;
using Cochief.Domain.Model;

namespace Cochief.Infrastructure.Presentation.Mappers;

public sealed class PresentationMappingProfile : Profile
{
    public PresentationMappingProfile()
    {
        CreateMap<User, UserResponseDto>();
    }
}
