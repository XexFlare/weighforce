using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class UserNameProfile : Profile
    {
        public UserNameProfile()
        {
            CreateMap<ApplicationUser, UserNameDto>();
            CreateMap<ApplicationUser, UserRoleDto>();
            CreateMap<UserSyncDto, ApplicationUser>();
            CreateMap<UserDto, ApplicationUser>();
            CreateMap<string, ApplicationUser>()
                .ForMember(dto => dto.Email, conf => conf.MapFrom(ol => ol));
            CreateMap<ApplicationUser, UserSyncDto>();
        }
    }
}