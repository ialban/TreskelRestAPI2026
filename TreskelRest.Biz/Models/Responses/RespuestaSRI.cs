namespace TreskelRest.Biz.Models.Responses
{
    public class RespuestaSRI
    {
        public string NumeroAutorizacion { get; set; }
        public DateTime? FechaAutorizacion { get; set; }
        public string Estado { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; }
    }
}
