using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class OsrProfile : Profile
    {
        public OsrProfile()
        {
            CreateMap<OsrData, OsrDetailsDTO>();
            CreateMap<OsrDetailsDTO, OsrData>();
        }
    }
}