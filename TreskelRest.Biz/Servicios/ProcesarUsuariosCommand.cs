using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Models.Requests;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;
using TreskelRest.Data.Models;
using static System.Net.WebRequestMethods;

namespace TreskelRest.Biz.Servicios
{
    public record ProcesarUsuariosCommand(UsuarioRequest usuario) : IRequest<UsuarioResponse>;

    public class ProcesarUsuariosHandler : ProcessHandler, IRequestHandler<ProcesarUsuariosCommand, UsuarioResponse>
    {
        private readonly ITreskelRepository _repo;
        private readonly IMapper _mapper;

        public ProcesarUsuariosHandler(
            ITreskelRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<UsuarioResponse> Handle(ProcesarUsuariosCommand request, CancellationToken cancellationToken)
        {
            if (request.usuario == null)
            {
                throw new ArgumentNullException(nameof(request.usuario));
            }

            UsuarioEmpresa _usuario = _mapper.Map<UsuarioEmpresa>(request.usuario);
            var resultado = await _repo.actualizaCliente(_usuario);

            UsuarioResponse comprobante = new UsuarioResponse
            {
                EmpresaId = request.usuario.EmpresaId,
                
            };

            return comprobante;
        }
    }
}
