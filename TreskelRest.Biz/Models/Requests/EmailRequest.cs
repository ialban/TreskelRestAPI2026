namespace TreskelRest.Biz.Models.Requests
{
    public class EmailRequest
    {
        public string origen { get; set; }
        public string destino { get; set; }
        public string asunto { get; set; }
        public string contenido { get; set; }
        public string nombreArchivo { get; set; }
        public byte[] archivoPdf { get; set; }
        public byte[] archivoXml { get; set; }
        public string token { get; set; }
    }
}
