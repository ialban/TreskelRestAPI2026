using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Mappers;
using TreskelRest.Biz.Models.Common;
using TreskelRest.Biz.Models.Correo;
using TreskelRest.Data;

namespace TreskelRest.Biz.Servicios
{
    public record ProcesarSendMailCommand(EmailPayload email, string empresaId);
    public class ProcesarSendMailHandler : ServiciosHandler
    {
        private readonly ISendEmail _sendMail;

        public ProcesarSendMailHandler(
            ITreskelRepository repo,
            ISendEmail sendMail,
            BizMapperProfile mapper): base(repo, mapper)
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
