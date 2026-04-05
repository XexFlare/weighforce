using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class ContractsProfile : Profile
    {
        public ContractsProfile()
        {
            CreateMap<Contract, ContractReadDto>();
            CreateMap<Contract, ContractNumDto>();
            CreateMap<Contract, ContractSyncDto>();
            CreateMap<ContractWriteDto, Contract>();
            CreateMap<ContractNumDto, Contract>();
            CreateMap<ContractReadDto, Contract>();
            CreateMap<SyncableDto, Contract>();
            CreateMap<IDto, Contract>()
            .ForMember(dto => dto.Id, conf => conf.MapFrom(ol => ol.Id));
        }
    }
}