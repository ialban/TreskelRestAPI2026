using System.Xml.Serialization;

namespace TreskelRest.API.Sri
{
    [XmlRoot(ElementName = "impuestos")]
    public class Impuestos
    {
        [XmlElement(ElementName = "impuesto")]
        public List<Impuesto> Impuesto { get; set; }
    }
}
