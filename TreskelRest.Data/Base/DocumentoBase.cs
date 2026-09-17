namespace TreskelRest.Data.Base
{
    public class DocumentoBase
    {
        public long? id { get; set; }
        public string secuencial { get; set; }
        public string claveAcceso { get; set; }
        public string ruc { get; set; }
        public string serial { get; set; }
        public string empresaid { get; set; }
        public string codigoLocal { get; set; }
        public string nombreLocal { get; set; }
        public string idCliente { get; set; }
        public string nombreCliente { get; set; }
        public string emailCliente { get; set; }
        public string data { get; set; }
        public byte[] bData { get; set; }
        public bool? procesado { get; set; }
        public bool? pdfGenerado { get; set; }
        public bool? enviadoCliente { get; set; }
        public string estadoEnvioMail { get; set; }
        public string environment { get; set; }
        public byte[] logotipo { get; set; }
        public string token { get; set; }
        public string email { get; set; }
        public string mailNombre { get; set; }
        public string estado { get; set; }
        public string estadoCodigo { get; set; }
        public DateTime fechaAutorizacion { get; set; }
        public string mensaje { get; set; }
        public string tipo { get; set; }
        public string pcName { get; set; }
        public string ipAddress { get; set; }
        public string claveDownload { get; set; }
        public string archivoPath { get; set; }
        public string archivoId { get; set; }
    }
}
