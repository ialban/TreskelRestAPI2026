using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Common;
using TreskelRest.API.Helpers;
using TreskelRest.Biz.Servicios;

namespace TreskelRest.API.Controllers
{
    [Route("[controller]")]
    public class NotaCreditoController : TreskelController
    {
        public NotaCreditoController(IConfiguration config, IMediator mediator, IMapper mapper) : base(config, mediator, mapper)
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
                Biz.Models.Documentos.NotaCredito _documento = _mapper.Map<Biz.Models.Documentos.NotaCredito>(_notacredito);

                notacredito = _mapper.Map<Biz.Models.NotaCredito>(payload);

                var res = await _mediator.Send(new ProcesarNotaCreditoCommand(notacredito, _documento, payload.empresaid, payload.bData));
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
                notacredito = _mapper.Map<Biz.Models.NotaCredito>(payload);

                var res = await _mediator.Send(new VerificarNotasCreditoCommand(notacredito, payload.empresaid));
                return Ok(res);
            }
            return Unauthorized();
        }
    }
}
