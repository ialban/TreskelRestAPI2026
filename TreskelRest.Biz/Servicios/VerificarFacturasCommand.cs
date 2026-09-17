using AutoMapper;
using MediatR;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Models;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;
using TreskelRest.Data.Models;

namespace TreskelRest.Biz.Servicios
{
    public record VerificarFacturasCommand(Factura factura, string empresaId) : IRequest<ComprobanteResponse>;
    public class VerificarFacturasHandler : IRequestHandler<VerificarFacturasCommand, ComprobanteResponse>
    {
        private readonly ITreskelRepository _repo;
        private readonly ISriWebService _sriService;
        private readonly IMapper _mapper;

        public VerificarFacturasHandler(
            ITreskelRepository repo,
            ISriWebService sriWebService,
            IMapper mapper)
        {
            _repo = repo;
            _sriService = sriWebService;
            _mapper = mapper;
        }

        public async Task<ComprobanteResponse> Handle(VerificarFacturasCommand request, CancellationToken cancellationToken)
        {
            if (request.factura == null)
            {
                throw new ArgumentNullException(nameof(request.factura));
            }

            // 1.- Obtenemos los datos de la fatura para la empresa
            Facturacion _factura = _mapper.Map<Facturacion>(request.factura);
            _factura.empresaid = request.empresaId;

            var factura = await _repo.obtenerFactura(_factura, "F");
            if (factura == null)
            {
                throw new ArgumentNullException(nameof(factura), $"No se encontró la factura");
            }

            ComprobanteResponse respuesta = new ComprobanteResponse
            {
                ClaveAcceso = request.factura.ClaveAcceso,
                Secuencial = request.factura.Secuencial,
                Estado = factura.estado,
                EstadoMail = factura.estadoEnvioMail != "E" ? false: true,
                Mensaje = factura.mensaje,
                Tipo = "F"
            };

            return respuesta;
        }
    }
}
