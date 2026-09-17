using TreskelRest.Biz.Models.Common;

namespace TreskelRest.Biz.Models
{
    public class NotaCredito: DocumentoBase
    {
        public string Ruc { get; set; }
        public string Secuencial { get; set; }
        public string NumeroFactura { get; set; }
        public string Autorizacion { get; set; }
        public string FechaAutorizacion { get; set; }
        public string Ambiente { get; set; }
        public string Emision { get; set; }
        public string ClaveAcceso { get; set; }
        public string RazonSocial { get; set; }
        public string DireccionMatriz { get; set; }
        public string DireccionLocal { get; set; }
        public string ObligadoContabilidad { get; set; }
        public string ContribuyenteEspecial { get; set; }
        public string ContribuyenteRimpe { get; set; }
        public string Cliente { get; set; }
        public string ClienteNid { get; set; }
        public string DireccionCliente { get; set; }
        public string FechaEmision { get; set; }

        //------------------------------------------------------------
        public string Subtotal12 { get; set; }
        public string Subtotal0 { get; set; }
        public string IVA { get; set; }
        public string Descuento { get; set; }
        public string Total { get; set; }
        //------------------------------------------------------------
        public List<NotaCreditoDetalle> NotaCreditoDetalles { get; set; } = new List<NotaCreditoDetalle>();
        //------------------------------------------------------------
        public List<InformacionAdicional> InformacionAdicional { get; set; }
    }
}
