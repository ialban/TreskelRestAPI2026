using TreskelRest.Biz.Mappers;
using TreskelRest.Biz.Models.Common;
using TreskelRest.Biz.Models.Requests;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;
using TreskelRest.Data.Models;

namespace TreskelRest.Biz.Servicios
{
    public record ProcesarUsuariosCommand(UsuarioRequest usuario);

    public class ProcesarUsuariosHandler : ServiciosHandler
    {
        public ProcesarUsuariosHandler(
            ITreskelRepository repo,
            BizMapperProfile mapper): base(repo, mapper)
        {

        }

        public async Task<UsuarioResponse> Handle(ProcesarUsuariosCommand request, CancellationToken cancellationToken)
        {
            if (request.usuario == null)
            {
                throw new ArgumentNullException(nameof(request.usuario));
            }

            UsuarioEmpresa _usuario = _mapper.UsuarioRequestToUsuario(request.usuario);
                
            var resultado = await _repo.actualizaCliente(_usuario);

            UsuarioResponse comprobante = new UsuarioResponse
            {
                EmpresaId = request.usuario.EmpresaId,
                
            };

            return comprobante;
        }
    }
}
