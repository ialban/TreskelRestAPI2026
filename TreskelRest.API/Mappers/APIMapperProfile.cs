using Riok.Mapperly.Abstractions;
using TreskelRest.API.Common;
using TreskelRest.API.Models.Mensajes;
using TreskelRest.API.Models.Usuarios;

namespace TreskelRest.API.Mappers
{
    [Mapper]
    public partial class APIMapperProfile
    {
        // ==========================================================================
        // 1. ➡️ MAPEO DE IDA: De FacturaModel a Biz.Models.Factura
        // ==========================================================================
        [MapProperty(nameof(FacturaModel.nombreCliente), nameof(Biz.Models.Factura.Cliente))]
        [MapProperty(nameof(FacturaModel.idCliente), nameof(Biz.Models.Factura.ClienteNid))]
        [MapProperty(nameof(FacturaModel.ruc), nameof(Biz.Models.Factura.Ruc))]
        [MapProperty(nameof(FacturaModel.claveAcceso), nameof(Biz.Models.Factura.ClaveAcceso))]
        [MapProperty(nameof(FacturaModel.emailCliente), nameof(Biz.Models.Factura.EmailCliente))]
        [MapProperty(nameof(FacturaModel.direccionLocal), nameof(Biz.Models.Factura.DireccionLocal))]
        [MapProperty(nameof(FacturaModel.nombreLocal), nameof(Biz.Models.Factura.RazonSocial))]
        [MapProperty(nameof(FacturaModel.secuencial), nameof(Biz.Models.Factura.Secuencial))]
        // Nota: Propiedades como 'empresaid', 'pcName', 'ipAddress', 'claveDownload' y 'serial' 
        // se mapean automáticamente si se llaman exactamente igual en ambos modelos.
        public partial Biz.Models.Factura ToBizFactura(FacturaModel source);

        // ==========================================================================
        // 2. ↩️ MAPEO INVERSO (Equivalente a .ReverseMap())
        // ==========================================================================
        [MapProperty(nameof(Biz.Models.Factura.Cliente), nameof(FacturaModel.nombreCliente))]
        [MapProperty(nameof(Biz.Models.Factura.ClienteNid), nameof(FacturaModel.idCliente))]
        [MapProperty(nameof(Biz.Models.Factura.Ruc), nameof(FacturaModel.ruc))]
        [MapProperty(nameof(Biz.Models.Factura.ClaveAcceso), nameof(FacturaModel.claveAcceso))]
        [MapProperty(nameof(Biz.Models.Factura.EmailCliente), nameof(FacturaModel.emailCliente))]
        [MapProperty(nameof(Biz.Models.Factura.DireccionLocal), nameof(FacturaModel.direccionLocal))]
        [MapProperty(nameof(Biz.Models.Factura.RazonSocial), nameof(FacturaModel.nombreLocal))]
        [MapProperty(nameof(Biz.Models.Factura.Secuencial), nameof(FacturaModel.secuencial))]
        public partial FacturaModel ToFacturaModel(Biz.Models.Factura source);


        public partial Biz.Models.Correo.EmailPayload ToBizEmailPayload(EmailPayload source);
        public partial EmailPayload ToAPIEmailPayload(Biz.Models.Correo.EmailPayload source);

        public partial Biz.Models.Correo.CuentaEmail ToBizCuentaEmail(CuentaEmail source);
        public partial CuentaEmail ToAPICuentaEmail(Biz.Models.Correo.CuentaEmail source);


        public partial Biz.Models.NotaCredito ToBizNotaCredito(NotaCreditoModel source);
        public partial NotaCreditoModel ToAPINotaCredito(Biz.Models.NotaCredito source);

        public partial Biz.Models.Documentos.Factura FacturaSriToFactura(Sri.Factura source);
        public partial Biz.Models.Documentos.NotaCredito NotaCreditoSriToNotaCredito(Sri.NotaCredito source);
        public partial Biz.Models.Documentos.Retencion RetencionSriToRetencion(Sri.Retencion source);


        public partial Biz.Models.NotaCreditoDetalle ToBizNotaCreditoDetalle(Sri.NotaCreditoDetalle source);
        public partial Sri.NotaCreditoDetalle ToAPINotaCreditoDetalle(Biz.Models.NotaCreditoDetalle source);



        [MapProperty(nameof(RetencionModel.ruc), nameof(Biz.Models.Retencion.Ruc))]
        [MapProperty(nameof(RetencionModel.claveAcceso), nameof(Biz.Models.Retencion.ClaveAcceso))]
        [MapProperty(nameof(RetencionModel.secuencial), nameof(Biz.Models.Retencion.Secuencial))]
        public partial Biz.Models.Retencion ToEntity(RetencionModel src);

        [MapProperty(nameof(Biz.Models.Retencion.Ruc), nameof(RetencionModel.ruc))]
        [MapProperty(nameof(Biz.Models.Retencion.ClaveAcceso), nameof(RetencionModel.claveAcceso))]
        [MapProperty(nameof(Biz.Models.Retencion.Secuencial), nameof(RetencionModel.secuencial))]
        public partial RetencionModel ToModel(Biz.Models.Retencion src);


        public partial Biz.Models.Requests.AuthenticationRequests ToBizAuthenticationRequest(AuthenticationRequest source);
        public partial AuthenticationRequest ToAPIAuthenticationRequest(Biz.Models.Requests.AuthenticationRequests source);

        public partial Biz.Models.Responses.AuthenticationResponse ToBizAuthenticationResponse(AuthenticationResponse source);
        public partial AuthenticationResponse ToAPIAuthenticationResponse(Biz.Models.Responses.AuthenticationResponse source);


        public partial Biz.Models.Correo.MensajeWs MessageWsToBizMessageWs(Models.Mensajes.MensajeWs source);

        public partial Biz.Models.Requests.UsuarioRequest UsuarioEmpresaToUsuarioRequest(Data.Models.UsuarioEmpresa source);

    }
}
