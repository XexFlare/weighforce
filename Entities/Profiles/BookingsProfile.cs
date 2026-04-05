using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class BookingsProfile : Profile
    {
        public BookingsProfile()
        {
            CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.Speed, source => source.MapFrom(source => source.VehicleType == "Truck" ? "" : (source.DeliveryNoteNumber != null && source.DeliveryNoteNumber != "") 
                    ? System.Math.Round(double.Parse(source.DeliveryNoteNumber), 2) + " km/h" 
                    : "-" ))
            .ForMember(dest => dest.DeliveryNoteNumber, source => source.MapFrom(source => source.VehicleType == "Truck" ? source.DeliveryNoteNumber : ""))
            .ForMember(dest => dest.WagonNo, source => source.MapFrom(source => source.VehicleType == "Truck" ? "" : source.PhoneNumber))
            .ForMember(dest => dest.PhoneNumber, source => source.MapFrom(source => source.VehicleType == "Truck" ? "" : source.PhoneNumber))
            .ForMember(dest => dest.DriverName, source => source.MapFrom(source => source.VehicleType != "Truck" ? source.VehicleType : source.DriverName))
            .ForMember(dest => dest.Transporter, source => source.MapFrom(source => source.OtherTransporter ?? source.Transporter.Name));
            CreateMap<Booking, CIDto>();
            CreateMap<CIDto, Booking>();
            CreateMap<Booking, BookingSyncDto>()
            .ForMember(dest => dest.TareUser, source => source.MapFrom(source => source.TareUser.Email))
            .ForMember(dest => dest.Branch, source => source.MapFrom(source => source.Branch.cId))
            .ForMember(dest => dest.Transporter, source => source.MapFrom(source => source.Transporter.cId))
            .ForMember(dest => dest.TransportInstruction, source => source.MapFrom(source => source.TransportInstruction.cId));
            CreateMap<BookingWriteDto, Booking>()
            .ForMember(dto => dto.Branch, conf => conf.MapFrom(ol => ol.Branch))
            .ForMember(dto => dto.Transporter, conf => conf.MapFrom(ol => ol.Transporter))
            .ForMember(dto => dto.TransportInstruction, conf => conf.MapFrom(ol => ol.TransportInstruction));
            CreateMap<BookingPostDto, Booking>();
            CreateMap<SyncableDto, Booking>();
            CreateMap<IDto, Booking>()
            .ForMember(dto => dto.Id, conf => conf.MapFrom(ol => ol.Id));
        }
    }
}