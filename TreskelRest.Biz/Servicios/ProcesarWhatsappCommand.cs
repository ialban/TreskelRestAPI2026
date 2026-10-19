using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Mappers;
using TreskelRest.Biz.Models.Common;
using TreskelRest.Biz.Models.Correo;
using TreskelRest.Data;

namespace TreskelRest.Biz.Servicios
{
    public record ProcesarWhatsappCommand(MensajeWs mensaje, string empresaId);
    public class ProcesarWhatsappHandler : ServiciosHandler
    {
        private readonly IWhatsapp _mensaje;
        
        public ProcesarWhatsappHandler(
            ITreskelRepository repo,
            IWhatsapp mensaje,
            BizMapperProfile mapper): base(repo, mapper)
        {
            _mensaje = mensaje;
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

            var result = _mensaje.EnviarMensajeAsync(request.mensaje, empresa.tokenWhatsapp);

            throw new NotImplementedException();
        }
    }
}
