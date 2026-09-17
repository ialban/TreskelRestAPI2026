namespace TreskelRest.Biz.Models.Documentos
{
    public class DetalleNotaCredito
    {
        public string CodigoInterno { get; set; }
        public string CodigoAuxiliar { get; set; }
        public string Descripcion { get; set; }
        public string Cantidad { get; set; }
        public string PrecioUnitario { get; set; }
        public string Descuento { get; set; }
        public string PrecioTotalSinImpuesto { get; set; }
        public DetallesAdicionales DetallesAdicionales { get; set; }
        public Impuestos Impuestos { get; set; }
    }
}
