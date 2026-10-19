using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Mappers;
using TreskelRest.Biz.Models.Common;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;
using TreskelRest.Data.Models;

namespace TreskelRest.Biz.Servicios
{
    public record VerificarNotasCreditoCommand(Biz.Models.NotaCredito notacredito, string empresaId) ;

    public class VerificarNotasCreditoHandler : ServiciosHandler
    {
        private readonly ISriWebService _sriService;

        public VerificarNotasCreditoHandler(
            ITreskelRepository repo,
            ISriWebService sriWebService,
            BizMapperProfile mapper): base(repo, mapper)
        {
            _repo = repo;
            _sriService = sriWebService;
            _mapper = mapper;
        }

        public async Task<ComprobanteResponse> Handle(VerificarNotasCreditoCommand request, CancellationToken cancellationToken)
        {
            if (request.notacredito == null)
            {
                throw new ArgumentNullException(nameof(request.notacredito));
            }

            // 1.- Obtenemos los datos de la fatura para la empresa
            NotaCredito _notacredito = _mapper.NotaCreditoToDto(request.notacredito);
                
            _notacredito.empresaid = request.empresaId;

            var notacredito = await _repo.obtenerNotaCredito(_notacredito, "C");
            if (notacredito == null)
            {
                throw new ArgumentNullException(nameof(notacredito), $"No se encontró la nota de crédito");
            }

            ComprobanteResponse respuesta = new ComprobanteResponse
            {
                ClaveAcceso = request.notacredito.ClaveAcceso,
                Secuencial = request.notacredito.Secuencial,
                Estado = notacredito.estado,
                EstadoMail = notacredito.estadoEnvioMail != "E" ? false : true,
                Mensaje = notacredito.mensaje,
                Tipo = "C"
            };

            return respuesta;
        }
    }
}
