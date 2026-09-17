using System.Text.Json;
using System.Text.Json.Serialization;
using TreskelRest.API.Common;


namespace TreskelRest.API.Models.Converters
{
    public class DocumentModelConverter : JsonConverter<DocumentoModel>
    {
        public override DocumentoModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);

            var json = document.RootElement;

            var resultado = new DocumentoModel();

            if (json.TryGetProperty("ventasec", out var ventaSec))
            {
                resultado.secuencial = ventaSec.GetString();
            }
            else if (json.TryGetProperty("retesec", out var reteSec))
            {
                resultado.secuencial = reteSec.GetString();
            }
            else if (json.TryGetProperty("ntcrsec", out var ntcrSec))
            {
                resultado.secuencial = ntcrSec.GetString();
            }
            else if (json.TryGetProperty("secuencial", out var sec))
            {
                resultado.secuencial = sec.GetString();
            }

            if (json.TryGetProperty("claveAcceso", out var clave))
                resultado.claveAcceso = clave.GetString();

            if (json.TryGetProperty("ruc", out var ruc))
                resultado.ruc = ruc.GetString();

            if (json.TryGetProperty("empresaid", out var empresa))
                resultado.empresaid = empresa.GetString();

            if (json.TryGetProperty("serial", out var serial))
                resultado.serial = serial.GetString();

            if (json.TryGetProperty("data", out var data))
                resultado.data = data.GetString();

            return resultado;
        }


        public override void Write(Utf8JsonWriter writer, DocumentoModel value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("secuencial", value.secuencial);
            writer.WriteString("claveAcceso", value.claveAcceso);
            writer.WriteString("ruc", value.ruc);

            writer.WriteEndObject();
        }
    }
}
