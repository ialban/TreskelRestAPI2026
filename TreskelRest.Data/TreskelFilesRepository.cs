using MongoDB.Driver;
using TreskelRest.Data.Models;

namespace TreskelRest.Data
{
    public class TreskelFilesRepository: ITreskelFilesRepository
    {
        private readonly IMongoCollection<EmpresaArchivos> _collection;

        public TreskelFilesRepository(TreskelFilesContext context)
        {
            var database = context.GetMongoDatabase();
            _collection = database.GetCollection<EmpresaArchivos>("EmpresaArchivos");
        }

        public async Task BorrarArchivoAsync(string idArchivo)
        {
            var filtro = Builders<EmpresaArchivos>.Filter.Eq(x => x.Id, idArchivo);
            await _collection.DeleteOneAsync(filtro);
        }

        public async Task<EmpresaArchivos> DescargarArchivoAsync(string idArchivo)
        {
            var filtro = Builders<EmpresaArchivos>.Filter.Eq(x => x.Id, idArchivo);
            var registro = await _collection.Find(filtro).FirstOrDefaultAsync();

            if (registro == null)
            {
                throw new Exception("El registro de archivos solicitado no existe.");
            }

            return registro;
        } 

        public async Task<string> SubirArchivoAsync(MongoRequest request)
        {
            var nuevoRegistro = new EmpresaArchivos
            {
                EmpresaId = request.empresaId,
                Autosri = request.claveSri,
                //-----------------------------------------------------------
                NombreArchivoXml = request.dataFile.nombreArchivo,
                TipoArchivoXml = request.dataFile.tipoArchivoXml,
                ArchivoXml = request.dataFile.archivoXml,
                //-----------------------------------------------------------
                NombreArchivoPdf = request.dataFile.nombreArchivo,
                TipoArchivoPdf = request.dataFile.tipoArchivoPdf,
                ArchivoPdf = request.dataFile.archivoPdf,
                //-----------------------------------------------------------
                FechaCreacion = DateTime.UtcNow
            };

            await _collection.InsertOneAsync(nuevoRegistro);
            return nuevoRegistro.Id;
        }
    }
}
