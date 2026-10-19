namespace TreskelRest.Biz.Interfaces
{
    public interface ISriComprobanteService
    {
        public byte[] GenerarFacturaPdf(Models.Factura factura, Models.Documentos.Factura documento, string empresaNombre, string direccionMatriz, bool createOnHd = true);
        public byte[] GenerarFacturaXml(Models.Factura factura, Models.Documentos.Factura documento, string empresaNombre, string direccionMatriz, string estado, bool createOnHd = true, byte[] xmlfile = null);

        public byte[] GenerarRetencionPdf(Models.Retencion retencion, Models.Documentos.Retencion documento, string empresaNombre, string direccionMatriz, bool createOnHd = true);
        public byte[] GenerarRetencionXml(Models.Retencion retencion, Models.Documentos.Retencion documento, string empresaNombre, string direccionMatriz, string estado, bool createOnHd = true, byte[] xmlfile = null);

        public byte[] GenerarNotaCreditoPdf(Models.NotaCredito notacredito, Models.Documentos.NotaCredito documento, string empresaNombre, string direccionMatriz, bool createOnHd = true);
        public byte[] GenerarNotaCreditoXml(Models.NotaCredito notacredito, Models.Documentos.NotaCredito documento, string empresaNombre, string direccionMatriz, string estado, bool createOnHd = true, byte[] xmlfile = null);
    }
}
