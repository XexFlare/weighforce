using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class UnitProfile : Profile
    {
        public UnitProfile()
        {
            CreateMap<Unit, UnitReadDto>();
            CreateMap<Unit, UnitSyncDto>();
            CreateMap<UnitWriteDto, Unit>();
            CreateMap<SyncableDto, Unit>();
            CreateMap<UnitReadDto, Unit>();
            CreateMap<IDto, Unit>()
            .ForMember(dto => dto.Id, conf => conf.MapFrom(ol => ol.Id));
        }
    }
}