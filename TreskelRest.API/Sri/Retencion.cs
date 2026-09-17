using System.Xml.Serialization;

namespace TreskelRest.API.Sri
{
    [XmlRoot(ElementName = "comprobanteRetencion", Namespace = "")]
    public class Retencion
    {
        [XmlElement(ElementName = "infoTributaria")]
        public InfoTributaria InfoTributaria { get; set; }
        [XmlElement(ElementName = "infoCompRetencion")]
        public InfoCompRetencion InfoCompRetencion { get; set; }
        [XmlElement(ElementName = "impuestos")]
        public ImpuestosRetencion Impuestos { get; set; }
        [XmlElement(ElementName = "maquinaFiscal")]
        public MaquinaFiscal MaquinaFiscal { get; set; }
        [XmlElement(ElementName = "infoAdicional")]
        public InfoAdicional InfoAdicional { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }
}
