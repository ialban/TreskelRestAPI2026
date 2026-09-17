namespace TreskelRest.Biz.Models.Documentos
{
    public class ImpuestoRetencion
    {
        public string Codigo { get; set; }
        public string CodigoRetencion { get; set; }
        public string BaseImponible { get; set; }
        public string PorcentajeRetener { get; set; }
        public string Valor { get; set; }
        public string CodDocSustento { get; set; }
        public string NumDocSustento { get; set; }
        public string FechaEmisionSustento { get; set; }
    }
}
