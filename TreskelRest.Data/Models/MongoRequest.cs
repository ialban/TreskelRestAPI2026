namespace TreskelRest.Data.Models
{
    public class MongoRequest
    {

        public string claveSri { get; set; }
        public string empresaId { get; set; }
        public MongoDataFile dataFile { get; set; }
    }

    public class MongoDataFile
    {
        public string nombreArchivo { get; set; }
        public string tipoArchivoXml { get; set; }
        public string tipoArchivoPdf { get; set; }
        public DateTime fechaSubida { get; set; }
        public byte[] archivoXml { get; set; }
        public byte[] archivoPdf { get; set; }
    }
}
