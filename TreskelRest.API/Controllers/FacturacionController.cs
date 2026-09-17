using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Common;
using TreskelRest.API.Helpers;
using TreskelRest.Biz.Servicios;

namespace TreskelRest.API.Controllers
{
    [Route("[controller]")]
    public class FacturacionController: TreskelController
    {
        public FacturacionController(IConfiguration config, IMediator mediator, IMapper mapper) :base(config, mediator, mapper)
        {

        }

        [HttpPost("declaracion")]
        public async Task<ActionResult> declaraFactura(FacturaModel payload, string token)
        {
            var valido = ValidateToken(token);

            if (valido != null)
            {
                Sri.Factura _factura = XMLHelper.Deserialize<Sri.Factura>(payload.data);

                Biz.Models.Factura factura = new Biz.Models.Factura();
                Biz.Models.Documentos.Factura _documento = _mapper.Map<Biz.Models.Documentos.Factura>(_factura);

                factura = _mapper.Map<Biz.Models.Factura>(payload);

                var res = await _mediator.Send(new ProcesarComprobanteCommand(factura, _documento, payload.empresaid, payload.bData));
                return Ok(res);
            }
            return Unauthorized();
        }

        [HttpPost("verificacion")]
        public async Task<ActionResult> verificaFactura(FacturaModel payload, string token)
        {
            var valido = ValidateToken(token);
            if (valido != null)
            {
                Biz.Models.Factura factura = new Biz.Models.Factura();
                factura = _mapper.Map<Biz.Models.Factura>(payload);

                var res = await _mediator.Send(new VerificarFacturasCommand(factura, payload.empresaid));
                return Ok(res);
            }
            return Unauthorized();
        }
    }
}
