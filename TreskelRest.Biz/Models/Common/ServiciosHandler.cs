using TreskelRest.Biz.Mappers;
using TreskelRest.Data;

namespace TreskelRest.Biz.Models.Common
{
    public class ServiciosHandler
    {
        protected ITreskelRepository _repo;
        protected BizMapperProfile _mapper;

        public ServiciosHandler(ITreskelRepository repo, BizMapperProfile mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

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
