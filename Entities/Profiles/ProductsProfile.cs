using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<Product, ProductReadDto>();
            CreateMap<Product, ProductSyncDto>()
                .ForMember(dest => dest.Unit, source => source.MapFrom(source => source.Unit.cId));
            CreateMap<ProductWriteDto, Product>();
            CreateMap<ProductReadDto, Product>();
            CreateMap<SyncableDto, Product>();
            CreateMap<IDto, Product>()
            .ForMember(dto => dto.Id, conf => conf.MapFrom(ol => ol.Id));
        }
    }
}