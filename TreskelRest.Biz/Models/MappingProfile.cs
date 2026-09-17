using AutoMapper;

namespace TreskelRest.Biz.Models
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Factura, Data.Models.Facturacion>()
                .ForMember(des => des.ruc, op => op.MapFrom(x => x.Ruc))
                .ForMember(des => des.secuencial, op => op.MapFrom(x => x.Secuencial))
                .ForMember(des => des.claveAcceso, op => op.MapFrom(x => x.ClaveAcceso))
                .ForMember(des => des.idCliente, op => op.MapFrom(x => x.ClienteNid))
                .ForMember(des => des.nombreCliente, op => op.MapFrom(x => x.Cliente))
                .ForMember(des => des.emailCliente, op => op.MapFrom(x => x.EmailCliente))
                .ForMember(des => des.pcName, op => op.MapFrom(x => x.pcName))
                .ForMember(des => des.ipAddress, op => op.MapFrom(x => x.ipAddress))
                .ForMember(des => des.claveDownload, op => op.MapFrom(x => x.claveDownload))
                .ForMember(des => des.codigoLocal, op => op.MapFrom(x => x.codigoLocal))
                .ForMember(des => des.pcName, op => op.MapFrom(x => x.pcName))
                .ForMember(des => des.empresaid, op => op.MapFrom(x => x.empresaid))
                .ReverseMap();

            CreateMap<Data.Models.Facturacion, Data.Models.Facturacion>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Retencion, Data.Models.Retencion>()
                .ReverseMap();

            CreateMap<NotaCredito, Data.Models.NotaCredito>()
                .ReverseMap();
        }
    }
}
