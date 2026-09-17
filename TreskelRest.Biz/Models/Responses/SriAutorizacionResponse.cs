using System.Xml.Serialization;

namespace TreskelRest.Biz.Models.Responses
{
    public class SriAutorizacionResponse
    {
        public string ClaveAcceso { get; set; }
        public int NumeroComprobantes { get; set; }
        public Autorizacion autorizacion { get; set; }
    }

    public class Autorizacion
    {
        [XmlElement(ElementName = "estado")]
        public string Estado { get; set; }

        [XmlElement(ElementName = "fechaAutorizacion")]
        public DateTime FechaAutorizacion { get; set; }

        [XmlElement(ElementName = "ambiente")]
        public string Ambiente { get; set; }

        [XmlElement(ElementName = "comprobante")]
        public string Comprobante { get; set; }

        [XmlArray("mensajes")]
        [XmlArrayItem("mensaje")]
        public List<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
    }
}
