using TreskelRest.Data.Base;
using TreskelRest.Data.Models;

namespace TreskelRest.Data
{
    public interface ITreskelRepository
    {
        Task<UsuarioEmpresa> login(UsuarioRequest usuario);
        Task<UsuarioEmpresa> getEmpresa(string empresaid);
        Task<UsuarioEmpresa> actualizaCliente(UsuarioEmpresa usuario);
        //---------------------------------------------------------------------------------------
        Task<Facturacion> grabarFactura(Facturacion facturacion, string fileId);
        Task<Facturacion> actualizaFactura(Facturacion facturacion);
        Task<Facturacion> obtenerFactura(Facturacion facturacion, string tipo);
        //---------------------------------------------------------------------------------------
        Task<Retencion> grabarRetencion(Retencion retencion, string fileId);
        Task<Retencion> actualizaRetencion(Retencion retencion);
        Task<Retencion> obtenerRetencion(Retencion retencion, string tipo);
        //---------------------------------------------------------------------------------------
        Task<NotaCredito> grabarNotaCredito(NotaCredito notacredito, string fileId);
        Task<NotaCredito> actualizaNotaCredito(NotaCredito notacredito);
        Task<NotaCredito> obtenerNotaCredito(NotaCredito notacredito, string tipo);
    }
}
