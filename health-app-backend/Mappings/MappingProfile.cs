using AutoMapper;
using health_app_backend.DTOs;
using health_app_backend.Migrations;
using health_app_backend.Models;

namespace health_app_backend.Mappings
{
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
            
            // Map DemographicBenchmark to DemographicBenchmarkDto
            CreateMap<DemographicBenchmark, DemographicBenchmarkDto>()
                .ForMember(dest => dest.AgeRange, opt => opt.MapFrom(src => src.AgeRange))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.TimeFrame, opt => opt.MapFrom(src => src.Timeframe))
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.AverageValue))
                .ForMember(dest => dest.RecommendedValue, opt => opt.MapFrom(src => src.RecommendedValue));
            
            // Map UserBenchmarkRecord to UserBenchmarkResponseDto
            CreateMap<UserBenchmarkRecordModel, UserBenchmarkResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserDataValue, opt => opt.MapFrom(src => src.DataValue))
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.DemographicBenchmark.AverageValue))
                .ForMember(dest => dest.RecommendedValue,
                    opt => opt.MapFrom(src => src.DemographicBenchmark.RecommendedValue))
                .ForMember(dest => dest.LocationName,
                    opt => opt.MapFrom(src => src.DemographicBenchmark.Location.CityName))
                .ForMember(dest => dest.TimeFrame, opt => opt.MapFrom(src => src.DemographicBenchmark.Timeframe))
                .ForMember(dest => dest.AgeRange, opt => opt.MapFrom(src => src.DemographicBenchmark.AgeRange))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.DemographicBenchmark.Gender))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
            
            // Map DemographicBenchmark to UserBenchmarkResponseDto
            CreateMap<DemographicBenchmark, UserBenchmarkResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore or map if necessary
                .ForMember(dest => dest.AgeRange, opt => opt.MapFrom(src => src.AgeRange))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.TimeFrame, opt => opt.MapFrom(src => src.Timeframe))
                .ForMember(dest => dest.AverageValue, opt => opt.MapFrom(src => src.AverageValue))
                .ForMember(dest => dest.RecommendedValue, opt => opt.MapFrom(src => src.RecommendedValue))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.CityName))
                .ForMember(dest => dest.UserDataValue, opt => opt.Ignore()); // UserDataValue not present in DemographicBenchmark
            
            // Map FriendRequestModel to FriendRequestReceivedDto
            CreateMap<FriendRequest, FriendRequestReceivedDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
                .ForMember(dest => dest.SenderUsername, opt => opt.MapFrom(src => src.Sender.Username))
                .ForMember(dest => dest.ReceiverId, opt => opt.MapFrom(src => src.ReceiverId))
                .ForMember(dest => dest.ReceivedAt, opt => opt.MapFrom(src => src.SentAt));
            
            // Map ClanJoinRequestModel to ClanInviteDto
            CreateMap<ClanJoinRequest, ClanInviteDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.ClanName, opt => opt.MapFrom(src => src.Clan.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.RequestedAt));
            
            // Map Clan to ClanSearchDto
            CreateMap<ClanSearchDto, Clan>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));
            
            // Map Clan to ClanDetailsDto
            CreateMap<Clan, ClanDetailsDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members));
            
            // Map ClanMember to ClanMemberDto
            CreateMap<ClanMember, ClanMemberDto>()
                .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.ClanId, opt => opt.MapFrom(src => src.ClanId.ToString()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));
        }
    }
}