namespace TreskelRest.Biz.Interfaces
{
    public interface ISriComprobanteService
    {
        public byte[] GenerarFacturaPdf(Models.Factura factura, Models.Documentos.Factura documento, string empresaNombre, string direccionMatriz, bool createOnHd = true);

        public byte[] GenerarRetencionPdf(Models.Retencion retencion, Models.Documentos.Retencion documento, string empresaNombre, string direccionMatriz, bool createOnHd = true);

        public byte[] GenerarNotaCreditoPdf(Models.NotaCredito notacredito, Models.Documentos.NotaCredito documento, string empresaNombre, string direccionMatriz, bool createOnHd = true);
    }
}
