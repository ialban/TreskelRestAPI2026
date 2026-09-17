using System.Xml.Serialization;

namespace TreskelRest.API.Sri
{
    [XmlRoot(ElementName = "detalles")]
    public class NotaCreditoDetalle
    {
        [XmlElement(ElementName = "detalle")]
        public List<DetalleNotaCredito> detalle { get; set; }
    }
}
