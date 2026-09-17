namespace TreskelRest.Biz.Models.Requests
{
    public class AuthenticationRequests
    {
        public string Ruc { get; set; }
        public string EmpresaId { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
    }
}
