using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Common;
using TreskelRest.API.Helpers;
using TreskelRest.API.Models.Mensajes;
using TreskelRest.API.Models.Usuarios;
using TreskelRest.Biz.Servicios;

namespace TreskelRest.API.Controllers
{
    [Route("[controller]")]
    public class MensajesController : TreskelController
    {
        public MensajesController(IConfiguration config, IMediator mediator, IMapper mapper) : base(config, mediator, mapper)
        {

        }

        [HttpPost("sendmail")]
        public async Task<IActionResult> sendMail(EmailPayload payload, string token)
        {
            var valido = ValidateToken(token);

            if (valido != null)
            {
                Biz.Models.Correo.EmailPayload email = _mapper.Map<Biz.Models.Correo.EmailPayload>(payload);
                string empresaId = payload.empresaId;

                var res = await _mediator.Send(new ProcesarSendMailCommand(email, empresaId));
                return Ok(res);

            }
            
            return Unauthorized();
        }

        [HttpPost("whatsapp")]
        public async Task<IActionResult> whatsApp(MensajeWs payload, string token)
        {
            var valido = ValidateToken(token);

            if (valido != null)
            {
                Biz.Models.Correo.MensajeWs mensaje = _mapper.Map<Biz.Models.Correo.MensajeWs>(payload);
                string empresaId = payload.empresaId;
                //var res = await _mediator.Send(new ProcesarSendMailCommand(mensaje, empresaId));
                //return Ok(res);
                return Ok(null);
            }
            
            return Unauthorized();
        }

        [HttpPost("sms")]
        public async Task<IActionResult> sendSMS(EmailPayload payload, string token)
        {
            var valido = ValidateToken(token);

            if (valido != null)
            {
                return Ok(null);
            }
            
            return Unauthorized();
        }
    }
}
