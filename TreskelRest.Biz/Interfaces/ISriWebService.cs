using TreskelRest.Biz.Models.Responses;

namespace TreskelRest.Biz.Interfaces
{
    public interface ISriWebService
    {
        Task<RespuestaSRI> validaDocumento(string environment, byte[] xml, string claveAcceso);
    }
}
