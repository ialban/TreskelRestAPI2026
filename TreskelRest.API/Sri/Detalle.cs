using System.Xml.Serialization;

namespace TreskelRest.API.Sri
{
    [XmlRoot(ElementName = "detalle")]
    public class Detalle
    {
        [XmlElement(ElementName = "codigoPrincipal")]
        public string CodigoPrincipal { get; set; }
        [XmlElement(ElementName = "codigoAuxiliar")]
        public string CodigoAuxiliar { get; set; }
        [XmlElement(ElementName = "descripcion")]
        public string Descripcion { get; set; }
        [XmlElement(ElementName = "cantidad")]
        public string Cantidad { get; set; }
        [XmlElement(ElementName = "precioUnitario")]
        public string PrecioUnitario { get; set; }
        [XmlElement(ElementName = "descuento")]
        public string Descuento { get; set; }
        [XmlElement(ElementName = "precioTotalSinImpuesto")]
        public string PrecioTotalSinImpuesto { get; set; }
        [XmlElement(ElementName = "detallesAdicionales")]
        public DetallesAdicionales DetallesAdicionales { get; set; }
        [XmlElement(ElementName = "impuestos")]
        public Impuestos Impuestos { get; set; }
    }

    [XmlRoot(ElementName = "detallesAdicionales")]
    public class DetallesAdicionales
    {
        [XmlElement(ElementName = "detAdicional")]
        public List<DetAdicional> DetAdicional { get; set; }
    }

    [XmlRoot(ElementName = "detAdicional")]
    public class DetAdicional
    {
        [XmlAttribute(AttributeName = "nombre")]
        public string Nombre { get; set; }
        [XmlAttribute(AttributeName = "valor")]
        public string Valor { get; set; }
    }
}
