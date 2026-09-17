using TreskelRest.Biz.Models.Requests;

namespace TreskelRest.Biz.Interfaces
{
    public interface ISendEmail
    {
        Task<bool> EnviarCorreoAsync(EmailRequest contenido, Models.Documentos.Factura factura);
        Task<bool> EnviarCorreoAsync(EmailRequest contenido, Models.Documentos.Retencion retencion);
        Task<bool> EnviarCorreoAsync(EmailRequest contenido, Models.Documentos.NotaCredito notacredito);
        Task<bool> EnviarCorreoGeneralAsync(Biz.Models.Correo.EmailPayload contenido);
    }
}
