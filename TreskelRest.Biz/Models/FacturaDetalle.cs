namespace TreskelRest.Biz.Models
{
    public class FacturaDetalle
    {
        public string codigo { get; set; }
        public string cantidad { get; set; }
        public string descripcion { get; set; }
        public string precioUnitario { get; set; }
        public string descuento { get; set; }
        public string precioTotal { get; set; }
    }
}
