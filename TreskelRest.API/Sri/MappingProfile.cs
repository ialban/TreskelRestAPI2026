using AutoMapper;
using TreskelRest.Biz.Models.Documentos;

namespace TreskelRest.API.Sri
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Factura, Biz.Models.Documentos.Factura>();
            CreateMap<InfoTributaria, Biz.Models.Documentos.InfoTributaria>();
            CreateMap<InfoFactura, Biz.Models.Documentos.InfoFactura>();
            CreateMap<InfoAdicional, Biz.Models.Documentos.InfoAdicional>();
            CreateMap<Detalles, Biz.Models.Documentos.Detalles>();
            CreateMap<Detalle, Biz.Models.Documentos.Detalle>();
            CreateMap<DetallesAdicionales, Biz.Models.Documentos.DetallesAdicionales>();
            CreateMap<DetAdicional, Biz.Models.Documentos.DetAdicional>();
            CreateMap<CampoAdicional, Biz.Models.Documentos.CampoAdicional>();
            CreateMap<Pagos, Biz.Models.Documentos.Pagos>();
            CreateMap<Pago, Biz.Models.Documentos.Pago>();
            CreateMap<TotalConImpuestos, Biz.Models.Documentos.TotalConImpuestos>();
            CreateMap<TotalImpuesto, Biz.Models.Documentos.TotalImpuesto>();
            CreateMap<Impuestos, Biz.Models.Documentos.Impuestos>();
            CreateMap<Impuesto, Biz.Models.Documentos.Impuesto>();

            CreateMap<Retencion, Biz.Models.Documentos.Retencion>();
            CreateMap<ImpuestosRetencion, Biz.Models.Documentos.ImpuestosRetencion>();
            CreateMap<InfoCompRetencion, Biz.Models.Documentos.InfoCompRetencion>();
            CreateMap<ImpuestoRetencion, Biz.Models.Documentos.ImpuestoRetencion>();
            CreateMap<MaquinaFiscal, Biz.Models.Documentos.MaquinaFiscal>();
        }
    }
}
