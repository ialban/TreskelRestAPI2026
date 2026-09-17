namespace TreskelRest.API.Common
{
    public class RespuestaError
    {
        public int StatusCode { get; set; }
        public bool EsError { get; set; }
        public string Mensaje { get; set; }
    }
}
