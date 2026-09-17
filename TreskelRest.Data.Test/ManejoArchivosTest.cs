using MongoDB.Driver;
using System.Text;
using TreskelRest.Data.Models;
using TreskelRest.Data.Test.Base;

namespace TreskelRest.Data.Test
{
    public class ManejoArchivosTest: MongoUTestBase
    {
        [Test]
        public async Task Upload()
        {
            // --- ARRANGE (Preparar los datos de prueba) ---
            byte[] bytesXmlSimulados = Encoding.UTF8.GetBytes("<factura><id>123</id></factura>");
            byte[] bytesPdfSimulados = Encoding.UTF8.GetBytes("%PDF-1.4 Contenido Binario Simulado");

            var payload = new MongoRequest
            {
                empresaId = "2C27EEE2-04D4-4F63-84AD-93535046D040",
                claveSri = "0102202601099271540500120010010000000011234567811",
                dataFile = new MongoDataFile // Ajusta el nombre de la clase interna si difiere
                {
                    nombreArchivo = "Factura_001_001_000000001",
                    tipoArchivoXml = "application/xml",
                    archivoXml = bytesXmlSimulados,
                    tipoArchivoPdf = "application/pdf",
                    archivoPdf = bytesPdfSimulados
                }
            };

            // --- ACT (Ejecutar el método del repositorio) ---
            string registroId = await _repo.SubirArchivoAsync(payload);

            // --- ASSERT (Verificar que se guardó correctamente) ---
            // 1. Validar que devolvió un ID de MongoDB (ObjectId de 24 caracteres hexadecimales)
            Assert.That(registroId, Is.Not.Null.Or.Empty);
            Assert.That(registroId.Length, Is.EqualTo(24));

            // 2. Recuperar el registro directamente de la base de datos de pruebas para auditarlo
            // Nota: Si ya creaste el método 'ObtenerRegistroPorIdAsync', úsalo aquí. 
            // De lo contrario, puedes consultar directamente la colección de prueba:
            var database = _context.GetMongoDatabase();
            var coleccionPrueba = database.GetCollection<EmpresaArchivos>("EmpresaArchivos");
            var registroGuardado = await coleccionPrueba.Find(x => x.Id == registroId).FirstOrDefaultAsync();

            // 3. Evaluaciones del registro único
            Assert.That(registroGuardado, Is.Not.Null, "El registro no se encontró en MongoDB.");
            Assert.That(registroGuardado.EmpresaId, Is.EqualTo(payload.empresaId));
            Assert.That(registroGuardado.Autosri, Is.EqualTo(payload.claveSri));
            Assert.That(registroGuardado.FechaCreacion, Is.LessThanOrEqualTo(DateTime.UtcNow));

            // 4. Verificación del archivo XML
            Assert.That(registroGuardado.NombreArchivoXml, Is.EqualTo(payload.dataFile.nombreArchivo));
            Assert.That(registroGuardado.TipoArchivoXml, Is.EqualTo(payload.dataFile.tipoArchivoXml));
            Assert.That(registroGuardado.ArchivoXml, Is.EqualTo(bytesXmlSimulados));

            // 5. Verificación del archivo PDF
            Assert.That(registroGuardado.NombreArchivoPdf, Is.EqualTo(payload.dataFile.nombreArchivo));
            Assert.That(registroGuardado.TipoArchivoPdf, Is.EqualTo(payload.dataFile.tipoArchivoPdf));
            Assert.That(registroGuardado.ArchivoPdf, Is.EqualTo(bytesPdfSimulados));
        }

        [Test]
        public async Task Descargar()
        {
            string idArchivo = "6a4454f03c57d37396e25633";

            EmpresaArchivos registro = await _repo.DescargarArchivoAsync(idArchivo);

            Assert.That(registro, Is.Not.Null.Or.Empty);
            Assert.That(registro.Id, Is.EqualTo(idArchivo));
        }

        [Test]
        public async Task Eliminar()
        {
            string idArchivo = "6a4454f03c57d37396e25633";

            await _repo.BorrarArchivoAsync(idArchivo);
        }
    }
}
