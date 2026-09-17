namespace TreskelRest.Biz.Models.Documentos
{
    public class Detalle
    {
        public string CodigoPrincipal { get; set; }
        public string CodigoAuxiliar { get; set; }
        public string Descripcion { get; set; }
        public string Cantidad { get; set; }
        public string PrecioUnitario { get; set; }
        public string Descuento { get; set; }
        public string PrecioTotalSinImpuesto { get; set; }
        public DetallesAdicionales DetallesAdicionales { get; set; }
        public Impuestos Impuestos { get; set; }
    }

    public class DetallesAdicionales
    {
        public List<DetAdicional> DetAdicional { get; set; }
    }

    public class DetAdicional
    {
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }
}
