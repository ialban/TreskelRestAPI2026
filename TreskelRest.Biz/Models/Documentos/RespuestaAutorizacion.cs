using System.Xml.Serialization;

namespace TreskelRest.Biz.Models.Documentos
{
    [XmlRoot(ElementName = "autorizacion")]
    public class RespuestaAutorizacion
    {
        [XmlElement(ElementName = "estado")]
        public string Estado { get; set; }

        [XmlElement(ElementName = "numeroAutorizacion")]
        public string NumeroAutorizacion { get; set; }

        [XmlElement(ElementName = "ambiente")]
        public string Ambiente { get; set; }

        [XmlElement(ElementName = "comprobante")]
        public string Comprobante { get; set; }
    }
}
