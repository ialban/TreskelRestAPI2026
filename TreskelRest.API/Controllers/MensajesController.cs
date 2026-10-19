using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Common;
using TreskelRest.API.Mappers;
using TreskelRest.API.Models.Mensajes;
using TreskelRest.Biz.Servicios;
using Wolverine;

namespace TreskelRest.API.Controllers
{
    [Route("[controller]")]
    public class MensajesController : TreskelController
    {
        public MensajesController(IConfiguration config, IMessageBus mediator, APIMapperProfile mapper) : base(config, mediator, mapper)
        {

        }

        [HttpPost("sendmail")]
        public async Task<IActionResult> sendMail(EmailPayload payload, string token)
        {
            var valido = ValidateToken(token);

            if (valido != null)
            {
                Biz.Models.Correo.EmailPayload email = _mapper.ToBizEmailPayload(payload);
                    
                string empresaId = payload.empresaId;

                var res = await _mediator.InvokeAsync<bool>(new ProcesarSendMailCommand(email, empresaId));
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
                Biz.Models.Correo.MensajeWs mensaje = _mapper.MessageWsToBizMessageWs(payload);
                    
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
