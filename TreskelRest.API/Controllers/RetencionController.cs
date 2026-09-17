using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Common;
using TreskelRest.API.Helpers;
using TreskelRest.Biz.Servicios;

namespace TreskelRest.API.Controllers
{
    [Route("[controller]")]
    public class RetencionController : TreskelController
    {
        public RetencionController(IConfiguration config, IMediator mediator, IMapper mapper) : base(config, mediator, mapper)
        {

        }

        [HttpPost("declaracion")]
        public async Task<ActionResult> declaraRetencion(RetencionModel payload, string token)
        {
            var valido = ValidateToken(token);

            if (valido != null)
            {
                Sri.Retencion _retencion = XMLHelper.Deserialize<Sri.Retencion>(payload.data);

                Biz.Models.Retencion retencion = new Biz.Models.Retencion();
                Biz.Models.Documentos.Retencion _documento = _mapper.Map<Biz.Models.Documentos.Retencion>(_retencion);

                retencion = _mapper.Map<Biz.Models.Retencion>(payload);

                var res = await _mediator.Send(new ProcesarRetencionCommand(retencion, _documento, payload.empresaid, payload.bData));
                return Ok(res);
            }
            return Unauthorized();
        }

        [HttpPost("verificacion")]
        public async Task<ActionResult> verificaRetencion(RetencionModel payload, string token)
        {
            var valido = ValidateToken(token);
            if (valido != null)
            {
                Biz.Models.Retencion retencion = new Biz.Models.Retencion();
                retencion = _mapper.Map<Biz.Models.Retencion>(payload);

                var res = await _mediator.Send(new VerificarRetencionesCommand(retencion, payload.empresaid));
                return Ok(res);
            }
            return Unauthorized();
        }
    }
}
