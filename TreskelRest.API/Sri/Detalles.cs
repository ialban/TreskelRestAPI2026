using System.Xml.Serialization;

namespace TreskelRest.API.Sri
{
    [XmlRoot(ElementName = "detalles")]
    public class Detalles
    {
        [XmlElement(ElementName = "detalle")]
        public List<Detalle> Detalle { get; set; }
    }
}
