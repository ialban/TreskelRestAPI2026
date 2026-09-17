using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Serialization;

namespace TreskelRest.API.Sri
{
    [XmlRoot(ElementName = "pagos")]
    public class Pagos
    {
        [XmlElement(ElementName = "pago")]
        public List<Pago> Pago { get; set; }
    }
}
