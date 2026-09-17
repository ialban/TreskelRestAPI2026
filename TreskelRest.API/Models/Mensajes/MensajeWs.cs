namespace TreskelRest.API.Models.Mensajes
{
    public class MensajeWs
    {
        public string empresaId { get; set; }
        public List<string> destino { get; set; }
        public string titulo { get; set; }
        public string mensaje { get; set; }
    }
}
