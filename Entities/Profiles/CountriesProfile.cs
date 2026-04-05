using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class CountriesProfile : Profile
    {
        public CountriesProfile()
        {
            CreateMap<Country, CountryReadDto>();
            CreateMap<Country, CountrySyncDto>();
            CreateMap<CountryReadDto, Country>();
            CreateMap<CountrySyncDto, Country>();
            CreateMap<CountryWriteDto, Country>();
            CreateMap<IDto, Country>()
            .ForMember(dto => dto.Id, conf => conf.MapFrom(ol => ol.Id));
        }
    }
}