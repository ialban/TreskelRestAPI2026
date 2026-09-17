namespace TreskelRest.Biz.Models
{
    public class RetencionDetalle
    {
        public string comprobante { get; set; }
        public string numeroFactura { get; set; }
        public string fechaEmision { get; set; }
        public string ejercicioFiscal { get; set; }
        public string baseImponible { get; set; }
        public string tipoImpuesto { get; set; }
        public string porcentajeRetencion { get; set; }
        public string valorRetenido { get; set; }
    }
}
