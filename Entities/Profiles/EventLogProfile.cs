using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class EventLogProfile : Profile
    {
        public EventLogProfile()
        {
            CreateMap<EventLog, EventLogDto>()
                .ForMember(dto => dto.User, conf => conf.MapFrom(ol => ol.User.Email));
            CreateMap<EventLogDto, EventLog>();
        }
    }
}