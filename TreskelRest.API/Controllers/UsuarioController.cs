using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Common;
using TreskelRest.API.Mappers;
using TreskelRest.API.Models.Usuarios;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Biz.Servicios;
using TreskelRest.Data.Models;
using Wolverine;

namespace TreskelRest.API.Controllers
{
    [Route("[controller]")]
    public class UsuarioController: TreskelController
    {
        public UsuarioController(IConfiguration config, IMessageBus mediator, APIMapperProfile mapper) : base(config, mediator, mapper)
        {

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest payload)
        {
            Biz.Models.Requests.AuthenticationRequests request = new Biz.Models.Requests.AuthenticationRequests();

            request = _mapper.ToBizAuthenticationRequest(payload);
                
            var res = await _mediator.InvokeAsync<Biz.Models.Responses.AuthenticationResponse>(new GetLoginQuery(request));

            var response = _mapper.ToAPIAuthenticationResponse(res);
                
            response.EmpresaId = payload.EmpresaId;
            response.Token = Utils.generaToken(response, TokenKey);

            return Ok(response);
        }

        [HttpPost("actualiza")]
        public async Task<ActionResult> grabaLogoCliente(UsuarioEmpresa payload, string token)
        {
            var valido = ValidateToken(token);
            if (valido != null)
            {
                var usuarioEmpresa = _mapper.UsuarioEmpresaToUsuarioRequest(payload);
                    
                var resultado = await _mediator.InvokeAsync<UsuarioResponse>(new ProcesarUsuariosCommand(usuarioEmpresa));

                return Ok(resultado);
            }
            return Unauthorized();
        }
    }
}
