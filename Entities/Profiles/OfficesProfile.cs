using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class OfficesProfile : Profile
    {
        public OfficesProfile()
        {
            CreateMap<Office, OfficeReadDto>()
                .ForMember(destination => destination.Logo, opt => opt.NullSubstitute("/logos/mfc.jpg"));
            CreateMap<OfficeWriteDto, Office>();
            CreateMap<DispatchOfficeWriteDto, Office>();
            CreateMap<OfficeNameDto, Office>();
            CreateMap<OfficeReadDto, Office>();
            CreateMap<OfficePostDto, Office>();
            CreateMap<SyncableDto, Office>();
            CreateMap<Office, OfficeSyncDto>()
                .ForMember(dest => dest.Unit, source => source.MapFrom(source => source.Unit.cId))
                .ForMember(dest => dest.Country, source => source.MapFrom(source => source.Country.cId));
            CreateMap<IDto, Office>()
            .ForMember(dto => dto.Id, conf => conf.MapFrom(ol => ol.Id));
            CreateMap<IDto, Branch>()
            .ForMember(dto => dto.Id, conf => conf.MapFrom(ol => ol.Id));
            CreateMap<Office, BranchedOfficeDto>();
            CreateMap<Branch, BranchNameDto>();
            CreateMap<Branch, BranchDto>();
            CreateMap<Branch, BranchSyncDto>()
                .ForMember(dest => dest.Office, source => source.MapFrom(source => source.Office.cId));
            CreateMap<IDto, Branch>()
                .ForMember(dto => dto.Id, conf => conf.MapFrom(ol => ol.Id));
            CreateMap<BranchDto, Branch>();
            CreateMap<BranchWriteDto, Branch>();
            CreateMap<SyncableDto, Branch>();
            CreateMap<long, Branch>()
            .ForMember(dto => dto.cId, conf => conf.MapFrom(ol => ol));

        }
    }
}