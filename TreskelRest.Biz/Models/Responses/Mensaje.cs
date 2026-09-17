using System.Xml.Serialization;

namespace TreskelRest.Biz.Models.Responses
{
    [XmlType("mensaje")]
    public class Mensaje
    {
        [XmlElement(ElementName = "identificador")]
        public int identificador { get; set; }
        [XmlElement(ElementName = "mensaje")]
        public string mensaje { get; set; }
        [XmlElement(ElementName = "informacionAdicional")]
        public string informacionAdicional { get; set; }
        [XmlElement(ElementName = "tipo")]
        public string tipo { get; set; }
    }
}
