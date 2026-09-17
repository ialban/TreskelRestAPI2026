using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace TreskelRest.Biz.Models.Correo
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

    public class EmailPayload
    {
        public CuentaEmail origen { get; set; }
        public List<CuentaEmail> destino { get; set; }
        public string asunto { get; set; }
        public string contenido { get; set; }
        public bool EsHtml { get; set; }
        public string token { get; set; }
        public EmailType tipo { get; set; } = EmailType.Transactional;
        public List<Attachment> attachments { get; set; }
    }

    public class CuentaEmail
    {
        public string email { get; set; }
        public string name { get; set; }
    }

    public class Attachment
    {
        public string filename { get; set; }
        public string content { get; set; }
        public string content_type { get; set; }
    }
}
