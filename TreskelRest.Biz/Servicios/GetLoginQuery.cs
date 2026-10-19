using TreskelRest.Biz.Mappers;
using TreskelRest.Biz.Models.Common;
using TreskelRest.Biz.Models.Requests;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;

namespace TreskelRest.Biz.Servicios
{
    public record GetLoginQuery(AuthenticationRequests payload);

    public class GetLoginHandler : ServiciosHandler 
    {
        public GetLoginHandler(ITreskelRepository repo, BizMapperProfile mapper): base(repo, mapper)
        {
            
        }

        public async Task<AuthenticationResponse> Handle(GetLoginQuery request, CancellationToken cancellationToken)
        {
            if (request.payload == null)
            {
                throw new ArgumentNullException(nameof(request.payload));
            }

            Data.Base.UsuarioRequest usuario = _mapper.AuthenticationRequestToUsuarioRequest(request.payload);

            var empresa = await _repo.login(usuario);

            if (empresa == null)
            {
                throw new UnauthorizedAccessException("Acceso no autorizado, Usuario o contraseña incorrectos");
            }

            if (empresa.Clave == request.payload.Clave) {
                var response = _mapper.UsuarioToAuthenticationResponse(empresa);

                return response;
            }

            throw new UnauthorizedAccessException("Acceso no autorizado, Usuario o Contraseña inválida");
        }
    }
}
