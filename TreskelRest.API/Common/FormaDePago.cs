namespace TreskelRest.API.Common
{
    public class FormaDePago
    {
        public int tipo { get; set; }
        public decimal valor { get; set; }
        public int plazo { get; set; }
        public int tiempo { get; set; }
    }
}
