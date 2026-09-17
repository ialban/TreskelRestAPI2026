using System.Text.Json.Serialization;

namespace TreskelRest.API.Common
{
    public class DocumentoModel
    {
        public long? id { get; set; }
        public string secuencial { get; set; }

        [JsonPropertyName("ventasec")]
        public string ventasec
        {
            set
            {
                secuencial = value;
            }
        }

        [JsonPropertyName("retesec")]
        public string retesec
        {
            set
            {
                secuencial = value;
            }
        }

        [JsonPropertyName("ntcrsec")]
        public string ntcrsec
        {
            set
            {
                secuencial = value;
            }
        }

        public string claveAcceso { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ruc { get; set; }
        public string empresaid { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? serial { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? codigoLocal { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? nombreLocal { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? direccionLocal { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public byte[]? bData { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? pcName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ipAddress { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? claveDownload { get; set; }
    }
}
