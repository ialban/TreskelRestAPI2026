using System.ComponentModel.DataAnnotations;

namespace TreskelRest.API.Models.Usuarios
{
    public class AuthenticationRequest
    {
        [Required]
        public string Ruc { get; set; }
        [Required]
        public string EmpresaId { get; set; }
        [Required]
        public string Usuario { get; set; }
        [Required]
        public string Clave { get; set; }
    }
}
