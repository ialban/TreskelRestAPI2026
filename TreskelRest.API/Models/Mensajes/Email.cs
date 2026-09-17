namespace TreskelRest.API.Models.Mensajes
{
    public enum EmailType
    {
        Transactional,
        Marketing,
        Notification,
        Newsletter,
        OTP,
        Invoice
    }

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

    public class EmailPayload
    {
        public string empresaId { get; set; }
        public CuentaEmail origen { get; set; }
        public List<CuentaEmail> destino { get; set; }
        public string asunto { get; set; }
        public string contenido { get; set; }
        public string? nombreArchivo { get; set; }
        public bool EsHtml { get; set; } = true;
        public string? token { get; set; }
        public EmailType? tipo { get; set; } = EmailType.Transactional;
        public List<Attachment>? attachments { get; set; }
    }

    public class CuentaEmail
    {
        public string email { get; set; }
        public string? name { get; set; }
    }

    public class Attachment
    {
        public string filename { get; set; }
        public string content { get; set; }
        public string content_type { get; set; }
    }
}
