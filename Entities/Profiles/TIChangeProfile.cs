using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class TIChangeProfile : Profile
    {
        public TIChangeProfile()
        {
            CreateMap<TIChange, TIChangeDto>()
                .ForMember(dto => dto.User, conf => conf.MapFrom(ol => ol.User.Email));
            CreateMap<TIChange, TIChangeReadDto>();
            CreateMap<TIChangeDto, TIChange>();
        }
    }
}