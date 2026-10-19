using Riok.Mapperly.Abstractions;
using TreskelRest.Biz.Models.Responses;

namespace TreskelRest.Biz.Mappers
{
    [Mapper]
    public partial class BizMapperProfile
    {
        // ==========================================
        // 1. Mapeo: Factura <-> Data.Models.Facturacion
        // ==========================================

        [MapProperty(nameof(Models.Factura.ClienteNid), nameof(Data.Models.Facturacion.idCliente))]
        [MapProperty(nameof(Models.Factura.Cliente), nameof(Data.Models.Facturacion.nombreCliente))]
        public partial Data.Models.Facturacion FacturaToDto(Models.Factura src);

        [MapProperty(nameof(Data.Models.Facturacion.idCliente), nameof(Models.Factura.ClienteNid))]
        [MapProperty(nameof(Data.Models.Facturacion.nombreCliente), nameof(Models.Factura.Cliente))]
        public partial Models.Factura DtoToFactura(Data.Models.Facturacion src);

        // ==========================================
        // 2. Mapeo: Clonación Condicional (Solo no nulos)
        // ==========================================

        // Mapperly aplica por defecto la regla de no sobrescribir con nulos si configuras una actualización
        public partial void UpdateFacturacion(Data.Models.Facturacion src, Data.Models.Facturacion dest);
        public partial void UpdateRetencion(Data.Models.Retencion src, Data.Models.Retencion dest);
        public partial void UpdateNotaCredito(Data.Models.NotaCredito src, Data.Models.NotaCredito dest);

        // ==========================================
        // 3. Mapeo: Retencion <-> Data.Models.Retencion
        // ==========================================

        public partial Data.Models.Retencion RetencionToDto(Models.Retencion src);
        public partial Models.Retencion DtoToRetencion(Data.Models.Retencion src);

        // ==========================================
        // 4. Mapeo: NotaCredito <-> Data.Models.NotaCredito
        // ==========================================

        public partial Data.Models.NotaCredito NotaCreditoToDto(Models.NotaCredito src);
        public partial Models.NotaCredito DtoToNotaCredito(Data.Models.NotaCredito src);


        public partial AuthenticationResponse UsuarioToAuthenticationResponse(Data.Models.UsuarioEmpresa src);

        public partial Data.Base.UsuarioRequest AuthenticationRequestToUsuarioRequest(Models.Requests.AuthenticationRequests src);


        public partial Data.Models.UsuarioEmpresa UsuarioRequestToUsuario(Models.Requests.UsuarioRequest src);

        public partial Data.Models.UsuarioEmpresa UsuarioRequestToUsuario(Models.Correo.MensajeWs src);
    }
}
