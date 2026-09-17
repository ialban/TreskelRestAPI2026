using AutoMapper;
using TreskelRest.API.Common;

namespace TreskelRest.API.Models.Retenciones
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RetencionModel, Biz.Models.Retencion>()
                .ForMember(dest => dest.Ruc, opt => opt.MapFrom(src => src.ruc))
                .ForMember(dest => dest.ClaveAcceso, opt => opt.MapFrom(src => src.claveAcceso))
                .ForMember(dest => dest.empresaid, opt => opt.MapFrom(src => src.empresaid))
                .ForMember(dest => dest.pcName, opt => opt.MapFrom(src => src.pcName))
                .ForMember(dest => dest.ipAddress, opt => opt.MapFrom(src => src.ipAddress))
                .ForMember(dest => dest.claveDownload, opt => opt.MapFrom(src => src.claveDownload))
                .ForMember(dest => dest.serial, opt => opt.MapFrom(src => src.serial))
                .ForMember(dest => dest.Secuencial, opt => opt.MapFrom(src => src.secuencial))
                .ReverseMap();

        }
    }
}
