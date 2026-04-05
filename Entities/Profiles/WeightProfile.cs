using System;
using AutoMapper;
using WeighForce.Dtos;
using WeighForce.Models;

namespace WeighForce.Profiles
{
    public class LocalT : IMemberValueResolver<object, object,  Nullable<DateTime>,  Nullable<DateTime>>
    {

        public  Nullable<DateTime> Resolve(object source, object destination, Nullable<DateTime> sourceMember,  Nullable<DateTime> destMember, ResolutionContext context)
        {
            var converted = sourceMember?.ToLocalTime();
            return converted;
        }
    }
    public class WeightProfile : Profile
    {
        public WeightProfile()
        {
            CreateMap<Weight, WeightDto>()
                .ForMember(destination => destination.TareAt, conf => conf
                    .MapFrom< LocalT, Nullable<DateTime>>(ol => ol.TareAt))
                .ForMember(destination => destination.GrossAt, conf => conf
                    .MapFrom< LocalT, Nullable<DateTime>>(ol => ol.GrossAt));
            CreateMap<WeightWriteDto, Weight>();
            CreateMap<Weight, WeightSyncDto>()
                .ForMember(dto => dto.Office, conf => conf.MapFrom(ol => ol.Office.cId))
                .ForMember(dto => dto.TareUser, conf => conf.MapFrom(ol => ol.TareUser.Email))
                .ForMember(dto => dto.GrossUser, conf => conf.MapFrom(ol => ol.GrossUser.Email));
            CreateMap<SyncableDto, Weight>();
        }
    }
}