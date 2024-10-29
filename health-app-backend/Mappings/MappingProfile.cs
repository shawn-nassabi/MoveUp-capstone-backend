using AutoMapper;
using health_app_backend.DTOs;
using health_app_backend.Models;

namespace health_app_backend.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map User to UserResponseDto
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.CityName));
        
        // Map UserCreateDto to User (for creating new users)
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.Location, opt => opt.Ignore());
        
        // Map HealthData to HealthDataResponseDto
        CreateMap<HealthData, HealthDataResponseDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.DataTypeName, opt => opt.MapFrom(src => src.Datatype.Name));

        // Map HealthDataCreateDto to HealthData (for creating new records)
        CreateMap<HealthDataCreateDto, HealthData>();
    }
}