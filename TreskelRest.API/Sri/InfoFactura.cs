using System.Xml.Serialization;

namespace TreskelRest.API.Sri
{
    [XmlRoot(ElementName = "infoFactura")]
    public class InfoFactura
    {
        [XmlElement(ElementName = "fechaEmision")]
        public string FechaEmision { get; set; }
        [XmlElement(ElementName = "dirEstablecimiento")]
        public string DirEstablecimiento { get; set; }
        [XmlElement(ElementName = "obligadoContabilidad")]
        public string ObligadoContabilidad { get; set; }
        [XmlElement(ElementName = "tipoIdentificacionComprador")]
        public string TipoIdentificacionComprador { get; set; }
        [XmlElement(ElementName = "guiaRemision")]
        public string GuiaRemision { get; set; }
        [XmlElement(ElementName = "razonSocialComprador")]
        public string RazonSocialComprador { get; set; }
        [XmlElement(ElementName = "identificacionComprador")]
        public string IdentificacionComprador { get; set; }
        [XmlElement(ElementName = "direccionComprador")]
        public string DireccionComprador { get; set; }
        [XmlElement(ElementName = "totalSinImpuestos")]
        public string TotalSinImpuestos { get; set; }
        [XmlElement(ElementName = "totalDescuento")]
        public string TotalDescuento { get; set; }
        [XmlElement(ElementName = "totalConImpuestos")]
        public TotalConImpuestos TotalConImpuestos { get; set; }
        [XmlElement(ElementName = "propina")]
        public string Propina { get; set; }
        [XmlElement(ElementName = "importeTotal")]
        public string ImporteTotal { get; set; }
        [XmlElement(ElementName = "moneda")]
        public string Moneda { get; set; }
        [XmlElement(ElementName = "pagos")]
        public Pagos Pagos { get; set; }
        [XmlElement(ElementName = "valorRetIva")]
        public string ValorRetIva { get; set; }
        [XmlElement(ElementName = "valorRetRenta")]
        public string ValorRetRenta { get; set; }
    }
}
