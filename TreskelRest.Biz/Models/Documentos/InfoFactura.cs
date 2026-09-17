namespace TreskelRest.Biz.Models.Documentos
{
    public class InfoFactura
    {
        public string FechaEmision { get; set; }
        public string DirEstablecimiento { get; set; }
        public string ObligadoContabilidad { get; set; }
        public string TipoIdentificacionComprador { get; set; }
        public string GuiaRemision { get; set; }
        public string RazonSocialComprador { get; set; }
        public string IdentificacionComprador { get; set; }
        public string DireccionComprador { get; set; }
        public string TotalSinImpuestos { get; set; }
        public string TotalDescuento { get; set; }
        public TotalConImpuestos TotalConImpuestos { get; set; }
        public string Propina { get; set; }
        public string ImporteTotal { get; set; }
        public string Moneda { get; set; }
        public Pagos Pagos { get; set; }
        public string ValorRetIva { get; set; }
        public string ValorRetRenta { get; set; }
    }
}
