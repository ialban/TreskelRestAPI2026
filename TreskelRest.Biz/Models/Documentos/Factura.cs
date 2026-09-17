namespace TreskelRest.Biz.Models.Documentos
{
    public class Factura
    {
        public InfoTributaria InfoTributaria { get; set; }
        public InfoFactura InfoFactura { get; set; }
        public Detalles Detalles { get; set; }
        public InfoAdicional InfoAdicional { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
    }
}
