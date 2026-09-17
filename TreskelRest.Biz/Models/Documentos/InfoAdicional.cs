namespace TreskelRest.Biz.Models.Documentos
{
    public class InfoAdicional
    {
        public List<CampoAdicional> CampoAdicional { get; set; }
    }

    public class CampoAdicional
    {
        public string Nombre { get; set; }
        public string Text { get; set; }
    }
}
