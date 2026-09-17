using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Common;
using TreskelRest.API.Models.Usuarios;
using TreskelRest.Biz.Servicios;
using TreskelRest.Data.Models;

namespace TreskelRest.API.Controllers
{
    [Route("[controller]")]
    public class UsuarioController: TreskelController
    {
        public UsuarioController(IConfiguration config, IMediator mediator, IMapper mapper) : base(config, mediator, mapper)
        {

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest payload)
        {
            Biz.Models.Requests.AuthenticationRequests request = new Biz.Models.Requests.AuthenticationRequests();

            request = _mapper.Map<Biz.Models.Requests.AuthenticationRequests>(payload);
            var res = await _mediator.Send(new GetLoginQuery(request));

            var response = _mapper.Map<API.Models.Usuarios.AuthenticationResponse>(res);
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
                var usuarioEmpresa = _mapper.Map<Biz.Models.Requests.UsuarioRequest>(payload);
                var resultado = await _mediator.Send(new ProcesarUsuariosCommand(usuarioEmpresa));

                return Ok(resultado);
            }
            return Unauthorized();
        }
    }
}
