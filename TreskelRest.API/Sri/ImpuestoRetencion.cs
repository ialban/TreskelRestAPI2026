using System.ComponentModel;
using System.Xml.Serialization;

namespace TreskelRest.API.Sri
{
    public class ImpuestoRetencion
    {
        [XmlElement(ElementName = "codigo")]
        [Description("Código asociado a la retención")]
        public string Codigo { get; set; }
        [XmlElement(ElementName = "codigoRetencion")]
        [Description("Código del SRI para la retención")]
        public string CodigoRetencion { get; set; }
        [Description("Valor de la Base Imponible de la retención")]
        [XmlElement(ElementName = "baseImponible")]
        public string BaseImponible { get; set; }
        [XmlElement(ElementName = "porcentajeRetener")]
        public string PorcentajeRetener { get; set; }
        [XmlElement(ElementName = "valorRetenido")]
        public string Valor { get; set; }
        [XmlElement(ElementName = "codDocSustento")]
        public string CodDocSustento { get; set; }
        [XmlElement(ElementName = "numDocSustento")]
        public string NumDocSustento { get; set; }
        [XmlElement(ElementName = "fechaEmisionDocSustento")]
        public string FechaEmisionSustento { get; set; }
    }
}
