namespace TreskelRest.Biz.Models.Responses
{
    public class ArchivoResponse
    {
        public byte[] Contenido { get; set; }

        public string NombreArchivo { get; set; }

        public string ContentType { get; set; }
    }
}
