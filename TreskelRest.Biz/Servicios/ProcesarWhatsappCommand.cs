using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Models.Correo;
using TreskelRest.Data;

namespace TreskelRest.Biz.Servicios
{
    public record ProcesarWhatsappCommand(MensajeWs mensaje, string empresaId) : IRequest<bool>;
    public class ProcesarWhatsappHandler : ProcessHandler, IRequestHandler<ProcesarWhatsappCommand, bool>
    {
        private readonly ITreskelRepository _repo;
        private readonly IWhatsapp _mensaje;
        private readonly IMapper _mapper;

        public ProcesarWhatsappHandler(
            ITreskelRepository repo,
            IWhatsapp mensaje,
            IMapper mapper)
        {
            _repo = repo;
            _mensaje = mensaje;
            _mapper = mapper;
        }

        public async Task<bool> Handle(ProcesarWhatsappCommand request, CancellationToken cancellationToken)
        {
            if (request.mensaje == null)
            {
                throw new ArgumentNullException(nameof(request.mensaje));
            }

            var empresa = await _repo.getEmpresa(request.empresaId);
            if (empresa == null)
            {
                throw new Exception($"No se encontró la empresa con ID: {request.empresaId}");
            }

            if (empresa.tokenWhatsapp == null)
            {
                throw new Exception($"Su cuenta no está habilitada para enviar mensajes a Whatsapp");
            }

            MensajeWs contenido = _mapper.Map<MensajeWs>(request.mensaje);

            var result = _mensaje.EnviarMensajeAsync(contenido, empresa.tokenWhatsapp);

            throw new NotImplementedException();
        }
    }
}
