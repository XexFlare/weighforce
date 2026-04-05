using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class DispatchsProfile : Profile
    {
        public DispatchsProfile()
        {
            CreateMap<Dispatch, DispatchReadDto>()
                .ForMember(dest => dest.VehicleType, source => source.MapFrom(source => source.Booking.VehicleType))
                .ForMember(dest => dest.FromOffice, source => source.MapFrom(source => source.Booking.TransportInstruction.FromLocation))
                .ForMember(dest => dest.ToOffice, source => source.MapFrom(source => source.Booking.TransportInstruction.ToLocation))
                .ForMember(dest => dest.Product, source => source.MapFrom(source => source.Booking.TransportInstruction.Product))
                .ForMember(dest => dest.Contract, source => source.MapFrom(source => source.Booking.TransportInstruction.Contract))
                .ForMember(dest => dest.InitialWeight, source => source.MapFrom(source => source.InitialWeight ?? new Weight { Tare = 0, Gross = 0 }))
                .ForMember(dest => dest.ReceivalWeight, source => source.MapFrom(source => source.ReceivalWeight ?? new Weight { Tare = 0, Gross = 0 }
                ));
            CreateMap<DispatchFirstWeightDto, DispatchReadDto>()
                .ForMember(dest => dest.VehicleType, source => source.MapFrom(source => source.Booking.VehicleType))
                .ForMember(dest => dest.FromOffice, source => source.MapFrom(source => source.Booking.TransportInstruction.FromLocation))
                .ForMember(dest => dest.ToOffice, source => source.MapFrom(source => source.Booking.TransportInstruction.ToLocation))
                .ForMember(dest => dest.Product, source => source.MapFrom(source => source.Booking.TransportInstruction.Product))
                .ForMember(dest => dest.Contract, source => source.MapFrom(source => source.Booking.TransportInstruction.Contract))
                .ForMember(dest => dest.InitialWeight, source => source.MapFrom(source => source.InitialWeight ?? new Weight { Tare = 0, Gross = 0 }))
                .ForMember(dest => dest.ReceivalWeight, source => source.MapFrom(source => source.ReceivalWeight ?? new Weight { Tare = 0, Gross = 0 }))
                .ForMember(dest => dest.FirstWeight, source => source.MapFrom(source => source.FirstWeight ?? new Weight { Tare = 0, Gross = 0 }
            ));
            CreateMap<Dispatch, DispatchFullSyncDto>();
            CreateMap<Dispatch, DispatchFirstWeightDto>();
            CreateMap<Dispatch, DispatchSyncDto>()
                .ForMember(dest => dest.Booking, source => source.MapFrom(source => source.Booking.cId))
                .ForMember(dest => dest.InitialWeight, source => source.MapFrom(source => source.InitialWeight.cId))
                .ForMember(dest => dest.ReceivalWeight, source => source.MapFrom(source => source.ReceivalWeight.cId));
            CreateMap<DispatchWriteDto, Dispatch>();
        }
    }
}