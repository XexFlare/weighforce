using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class TIsProfile : Profile
    {
        public TIsProfile()
        {
            CreateMap<TransportInstruction, TIReadDto>();
            CreateMap<TransportInstruction, CIDto>();
            CreateMap<CIDto, TransportInstruction>();
            CreateMap<TransportInstruction, TISyncDto>()
                .ForMember(dest => dest.Contract, source => source.MapFrom(source => source.Contract.cId))
                .ForMember(dest => dest.Product, source => source.MapFrom(source => source.Product.cId))
                .ForMember(dest => dest.FromLocation, source => source.MapFrom(source => source.FromLocation.cId))
                .ForMember(dest => dest.ToLocation, source => source.MapFrom(source => source.ToLocation.cId));
            CreateMap<TIWriteDto, TransportInstruction>();
            CreateMap<TIPutDto, TransportInstruction>()
                .ForMember(dest => dest.Contract, source => source.MapFrom(source => source.Contract))
                .ForMember(dest => dest.Product, source => source.MapFrom(source => source.Product))
                .ForMember(dest => dest.FromLocation, source => source.MapFrom(source => source.FromLocation))
                .ForMember(dest => dest.ToLocation, source => source.MapFrom(source => source.ToLocation));
            CreateMap<SyncableDto, TransportInstruction>();
            CreateMap<IDto, TransportInstruction>()
            .ForMember(dto => dto.Id, conf => conf.MapFrom(ol => ol.Id));
            CreateMap<long, TransportInstruction>()
            .ForMember(dto => dto.cId, conf => conf.MapFrom(ol => ol));
        }
    }
}