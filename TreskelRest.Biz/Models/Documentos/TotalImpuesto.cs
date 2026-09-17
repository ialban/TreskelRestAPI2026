namespace TreskelRest.Biz.Models.Documentos
{
    public class TotalImpuesto
    {
        public string Codigo { get; set; }
        public string CodigoPorcentaje { get; set; }
        public string DescuentoAdicional { get; set; }
        public string BaseImponible { get; set; }
        public string Tarifa { get; set; }
        public string Valor { get; set; }

        public string DecoPorcentajeImpuesto()
        {
            switch (CodigoPorcentaje)
            {
                case "2":
                    return "12";
                case "3":
                    return "14";
                case "4":
                    return "15";
                case "5":
                    return "5";
                case "8":
                    return "8";
                case "10":
                    return "13";
                case "0":
                case "6":
                case "7":
                default:
                    return "0";
            }
        }
    }
}
