namespace TreskelRest.Biz.Models.Documentos
{
    public class Retencion
    {
        public InfoTributaria InfoTributaria { get; set; }
        public InfoCompRetencion InfoCompRetencion { get; set; }
        public ImpuestosRetencion Impuestos { get; set; }
        public MaquinaFiscal MaquinaFiscal { get; set; }
        public InfoAdicional InfoAdicional { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
    }
}
