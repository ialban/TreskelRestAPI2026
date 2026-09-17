namespace TreskelRest.Biz.Models.Documentos
{
    public class Impuesto
    {
        public string Codigo { get; set; }
        public string CodigoPorcentaje { get; set; }
        public string Tarifa { get; set; }
        public string BaseImponible { get; set; }
        public string Valor { get; set; }
    }
}
