using System.Xml.Serialization;

namespace TreskelRest.Biz.Models.Responses
{
    public class SriValidacionResponse
    {
        public string estado { get; set; }
        public Comprobante comprobante { get; set; }
    }

    [XmlRoot(ElementName = "comprobante")]
    public class Comprobante
    {
        [XmlElement(ElementName = "claveAcceso")]
        public string ClaveAcceso { get; set; }

        [XmlArray("mensajes")]
        [XmlArrayItem("mensaje")]
        public List<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
    }
}
