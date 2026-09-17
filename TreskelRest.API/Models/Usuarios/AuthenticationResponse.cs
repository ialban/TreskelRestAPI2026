namespace TreskelRest.API.Models.Usuarios
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string Ruc { get; set; }
        public string EmpresaId { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Token { get; set; }
        public string Environment { get; set; }
    }
}
