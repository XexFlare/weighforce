using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class OfficeCountryProfile : Profile
    {
        public OfficeCountryProfile()
        {
            CreateMap<Country, CountryReadDto>();
            CreateMap<CountryWriteDto, Country>();
        }
    }
}