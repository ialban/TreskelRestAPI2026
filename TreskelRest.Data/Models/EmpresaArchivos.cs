using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TreskelRest.Data.Models
{
    public class EmpresaArchivos
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string EmpresaId { get; set; }
        public string Autosri { get; set; }
        public DateTime FechaCreacion { get; set; }

        public byte[] ArchivoXml { get; set; }
        public string NombreArchivoXml { get; set; }
        public string TipoArchivoXml { get; set; }

        public byte[] ArchivoPdf { get; set; }
        public string NombreArchivoPdf { get; set; }
        public string TipoArchivoPdf { get; set; }
    }
}
