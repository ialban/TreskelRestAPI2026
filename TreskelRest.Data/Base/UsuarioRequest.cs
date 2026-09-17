namespace TreskelRest.Data.Base
{
    public class UsuarioRequest
    {
        public string Usuario { get; set; }
        public string Ruc { get; set; }
        public string EmpresaId { get; set; }
        public bool usaMongoDb { get; set; }
        public bool creaOnHd { get; set; }
        public string ambiente { get; set; }
        public string email { get; set; }
        public byte[]? Logotipo { get; set; }
    }
}
