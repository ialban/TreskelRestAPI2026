using AutoMapper;
using MediatR;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Models.Correo;
using TreskelRest.Data;

namespace TreskelRest.Biz.Servicios
{
    public record ProcesarSendMailCommand(EmailPayload email, string empresaId) : IRequest<bool>;
    public class ProcesarSendMailHandler : ProcessHandler, IRequestHandler<ProcesarSendMailCommand, bool>
    {
        private readonly ITreskelRepository _repo;
        private readonly ISendEmail _sendMail;
        private readonly IMapper _mapper;

        public ProcesarSendMailHandler(
            ITreskelRepository repo,
            ISendEmail sendMail,
            IMapper mapper)
        {
            _repo = repo;
            _sendMail = sendMail;
            _mapper = mapper;
        }

        public async Task<bool> Handle(ProcesarSendMailCommand request, CancellationToken cancellationToken)
        {
            if (request.email == null)
            {
                throw new ArgumentNullException(nameof(request.email));
            }

            // Se obtienen los datos del cliente
            var empresa = await _repo.getEmpresa(request.empresaId);
            if (empresa == null)
            {
                throw new ArgumentNullException(nameof(empresa), $"No se encontró la empresa con ID: {request.empresaId}");
            }

            // Se realiza el envío del correo
            EmailPayload contenido = new EmailPayload
            {
                token = empresa.tokenEmail,
                origen = request.email.origen,
                destino = request.email.destino,
                asunto = request.email.asunto,
                contenido = request.email.contenido,
                EsHtml = request.email.EsHtml
            };

            var correo = await _sendMail.EnviarCorreoGeneralAsync(contenido);

            return true;
        }
    }
}
