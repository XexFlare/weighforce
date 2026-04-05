using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class CountryOfficesProfile : Profile
    {
        public CountryOfficesProfile()
        {
            CreateMap<Office, OfficeNameDto>();
            CreateMap<OfficeWriteDto, Office>();
        }
    }
}