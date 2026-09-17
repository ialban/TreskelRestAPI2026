using AutoMapper;
using MediatR;
using TreskelRest.Biz.Models.Requests;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;

namespace TreskelRest.Biz.Servicios
{
    public record GetLoginQuery(AuthenticationRequests payload) : IRequest<AuthenticationResponse>;

    public class GetLoginHandler : IRequestHandler<GetLoginQuery, AuthenticationResponse>
    {
        private readonly ITreskelRepository _repo;
        private IMapper _mapper;

        public GetLoginHandler(ITreskelRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<AuthenticationResponse> Handle(GetLoginQuery request, CancellationToken cancellationToken)
        {
            if (request.payload == null)
            {
                throw new ArgumentNullException(nameof(request.payload));
            }

            Data.Base.UsuarioRequest usuario = _mapper.Map<Data.Base.UsuarioRequest>(request.payload);

            var empresa = await _repo.login(usuario);

            if (empresa == null)
            {
                throw new UnauthorizedAccessException("Acceso no autorizado, Usuario o contraseña incorrectos");
            }

            if (empresa.Clave == request.payload.Clave) { 
                var response = _mapper.Map<AuthenticationResponse>(empresa);

                return response;
            }

            throw new UnauthorizedAccessException("Acceso no autorizado, Usuario o Contraseña inválida");
        }
    }
}
