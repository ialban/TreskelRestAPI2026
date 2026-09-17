using TreskelRest.Data.Base;

namespace TreskelRest.Data.Models
{
    public class UsuarioEmpresa: UsuarioBase
    {
        public string Ruc { get; set; }
        public string EmpresaId { get; set; } // GUID
        public string Nombre { get; set; }
        public bool Bloqueado { get; set; }
        public bool Activo { get; set; }
        public bool UsaMongoDB { get; set; } = false;
        public bool CreateOnHd { get; set; } = false;
        public string Environment { get; set; }
        public byte[] Logotipo { get; set; }
        public string email { get; set; }

        public string tokenEmail { get; set; }
        public string tokenWhatsapp { get; set; }
        public string tokenSms { get; set; }
        public string cuentaws { get; set; }
        public string phonews { get; set; }
        public string cuentasms { get; set; }
    }
}
