namespace TreskelRest.Biz.Models.Documentos
{
    public class NotaCredito
    {
        public InfoTributaria InfoTributaria { get; set; }

        public InfoNotaCredito InfoNotaCredito { get; set; }

        public NotaCreditoDetalle Detalles { get; set; }
        public InfoAdicional InfoAdicional { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
    }
}
