namespace TreskelRest.Data.Models
{
    public class Resultado
    {
        public long id { get; set; }
        public string empresaid { get; set; }
        public string secuencial { get; set; }
        public string serial { get; set; }
        public byte[] xmlfile { get; set; }
        public byte[] pdffile { get; set; }
        public string estado { get; set; }
        public string estadomail { get; set; }
        public string mensaje { get; set; }

    }
}
