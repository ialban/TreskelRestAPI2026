using TreskelRest.Data.Models;

namespace TreskelRest.Data
{
    public interface ITreskelFilesRepository
    {
        Task<string> SubirArchivoAsync(MongoRequest request);
        Task<EmpresaArchivos> DescargarArchivoAsync(string idArchivo);
        Task BorrarArchivoAsync(string idArchivo);
    }
}
