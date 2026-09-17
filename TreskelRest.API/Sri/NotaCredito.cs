using System.Xml.Serialization;

namespace TreskelRest.API.Sri
{
    [XmlRoot(ElementName = "notaCredito")]
    public class NotaCredito
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id;

        [XmlAttribute(AttributeName = "version")]
        public string Version;

        [XmlElement(ElementName = "infoTributaria", Order = 1)]
        public InfoTributaria InfoTributaria { get; set; }

        [XmlElement(ElementName = "infoNotaCredito", Order = 2)]
        public InfoNotaCredito InfoNotaCredito;

        [XmlElement(ElementName = "detalles", Order = 3)]
        public NotaCreditoDetalle Detalles { get; set; }
        [XmlElement(ElementName = "infoAdicional", Order = 5)]
        public InfoAdicional InfoAdicional { get; set; }
    }
}
