namespace TreskelRest.Biz.Servicios
{
    public class ProcessHandler
    {
        protected string DecodificaEstadosSRI(string estado)
        {
            return estado switch
            {
                "AUTORIZADO" => "A",
                "NOPROCESADO" => "N",
                "DEVUELTA" => "D",
                "PENDIENTE" => "P",
                "VALIDADO" => "V",
                _ => estado,
            };
        }
    }
}
