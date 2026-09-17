using System.Xml.Serialization;

namespace TreskelRest.API.Sri
{
    [XmlRoot(ElementName = "impuestos")]
    public class ImpuestosRetencion
    {
        [XmlElement(ElementName = "impuesto")]
        public List<ImpuestoRetencion> Impuesto { get; set; }
    }
}
