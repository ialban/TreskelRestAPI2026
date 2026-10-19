using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Common;
using TreskelRest.API.Helpers;
using TreskelRest.API.Mappers;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Biz.Servicios;
using Wolverine;

namespace TreskelRest.API.Controllers
{
    [Route("[controller]")]
    public class FacturacionController: TreskelController
    {
        public FacturacionController(IConfiguration config, IMessageBus mediator, APIMapperProfile mapper) :base(config, mediator, mapper)
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
                Biz.Models.Documentos.Factura _documento = _mapper.FacturaSriToFactura(_factura);

                factura = _mapper.ToBizFactura(payload);

                var res = await _mediator.InvokeAsync<ComprobanteResponse>(new ProcesarComprobanteCommand(factura, _documento, payload.empresaid, payload.bData));
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
                factura = _mapper.ToBizFactura(payload);

                var res = await _mediator.InvokeAsync<ComprobanteResponse>(new VerificarFacturasCommand(factura, payload.empresaid));
                return Ok(res);
            }
            return Unauthorized();
        }
    }
}
