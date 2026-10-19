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
    public class NotaCreditoController : TreskelController
    {
        public NotaCreditoController(IConfiguration config, IMessageBus mediator, APIMapperProfile mapper) : base(config, mediator, mapper)
        {

        }

        [HttpPost("declaracion")]
        public async Task<ActionResult> declaraNotaCredito(NotaCreditoModel payload, string token)
        {
            var valido = ValidateToken(token);

            if (valido != null)
            {
                Sri.NotaCredito _notacredito = XMLHelper.Deserialize<Sri.NotaCredito>(payload.data);

                Biz.Models.NotaCredito notacredito = new Biz.Models.NotaCredito();
                Biz.Models.Documentos.NotaCredito _documento = _mapper.NotaCreditoSriToNotaCredito(_notacredito);

                notacredito = _mapper.ToBizNotaCredito(payload);

                var res = await _mediator.InvokeAsync<ComprobanteResponse>(new ProcesarNotaCreditoCommand(notacredito, _documento, payload.empresaid, payload.bData));
                return Ok(res);
            }
            return Unauthorized();
        }

        [HttpPost("verificacion")]
        public async Task<ActionResult> verificaNotaCretido(NotaCreditoModel payload, string token)
        {
            var valido = ValidateToken(token);
            if (valido != null)
            {
                Biz.Models.NotaCredito notacredito = new Biz.Models.NotaCredito();
                notacredito = _mapper.ToBizNotaCredito(payload);

                var res = await _mediator.InvokeAsync<ComprobanteResponse>(new VerificarNotasCreditoCommand(notacredito, payload.empresaid));
                return Ok(res);
            }
            return Unauthorized();
        }
    }
}
