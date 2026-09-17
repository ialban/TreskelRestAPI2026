using System;
using System.Collections.Generic;
using System.Text;

namespace TreskelRest.Biz.Models.Requests
{
    public class UsuarioRequest
    {
        public string Ruc { get; set; }
        public string EmpresaId { get; set; }
        public string Nombre { get; set; }
        public bool Bloqueado { get; set; }
        public bool Activo { get; set; }
        public bool UsaMongoDB { get; set; } = false;
        public bool CreateOnHd { get; set; } = false;
        public string Environment { get; set; }
        public byte[] Logotipo { get; set; }
        public string email { get; set; }
    }
}
