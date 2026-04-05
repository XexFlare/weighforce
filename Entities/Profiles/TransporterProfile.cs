using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class TransportersProfile : Profile
    {
        public TransportersProfile()
        {
            CreateMap<Transporter, TransporterDto>();
            CreateMap<Transporter, TransporterSyncDto>();
            CreateMap<TransporterWriteDto, Transporter>();
            CreateMap<TransporterPutDto, Transporter>();
            CreateMap<TransporterDto, Transporter>();
            CreateMap<SyncableDto, Transporter>();
            CreateMap<IDto, Transporter>()
            .ForMember(dto => dto.Id, conf => conf.MapFrom(ol => ol.Id));
            CreateMap<long, Transporter>()
            .ForMember(dto => dto.cId, conf => conf.MapFrom(ol => ol));
        }
    }
}