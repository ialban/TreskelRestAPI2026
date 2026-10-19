using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Mappers;
using TreskelRest.Biz.Models.Common;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;
using TreskelRest.Data.Models;

namespace TreskelRest.Biz.Servicios
{
    public record VerificarRetencionesCommand(Models.Retencion retencion, string empresaId);
    public class VerificarRetencionesHandler : ServiciosHandler
    {
        private readonly ISriWebService _sriService;

        public VerificarRetencionesHandler(
            ITreskelRepository repo,
            ISriWebService sriWebService,
            BizMapperProfile mapper): base(repo, mapper)
        {
            _sriService = sriWebService;
        }

        public async Task<ComprobanteResponse> Handle(VerificarRetencionesCommand request, CancellationToken cancellationToken)
        {
            if (request.retencion == null)
            {
                throw new ArgumentNullException(nameof(request.retencion));
            }

            // 1.- Obtenemos los datos de la fatura para la empresa
            Retencion _retencion = _mapper.RetencionToDto(request.retencion);

            _retencion.empresaid = request.empresaId;

            var retencion = await _repo.obtenerRetencion(_retencion, "R");
            if (retencion == null)
            {
                throw new ArgumentNullException(nameof(retencion), $"No se encontró la retención");
            }

            ComprobanteResponse respuesta = new ComprobanteResponse
            {
                ClaveAcceso = request.retencion.ClaveAcceso,
                Secuencial = request.retencion.Secuencial,
                Estado = retencion.estado,
                EstadoMail = retencion.estadoEnvioMail != "E" ? false : true,
                Mensaje = retencion.mensaje,
                Tipo = "R"
            };

            return respuesta;
        }
    }
}
