using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Text;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Models.Correo;
using TreskelRest.Biz.Models.Requests;

namespace TreskelRest.Biz.Procesos
{
    public class SendEmail: ISendEmail
    {
        private readonly IConfiguration _config;
        private bool usaHtml = false;

        public SendEmail(IConfiguration config)
        {
            _config = config;
        }

        // 1. MÉTODO CENTRALIZADO GENERAL (Hace todo el trabajo pesado de RestSharp)
        private async Task<bool> EnviarCorreoBaseAsync(EmailRequest contenido, string cuerpoMensaje)
        {
            // Variables locales para evitar colisiones entre hilos asíncronos
            var host = _config.GetSection("EmailSettings:host").Value;
            bool localUsaHtml = bool.TryParse(_config.GetSection("EmailSettings:UsaHtml").Value, out bool result) ? result : true;

            // Nota: Lo ideal para alto rendimiento es que '_client' sea inyectado en el constructor de la clase
            var client = new RestClient(host);
            var request = new RestRequest(host, Method.Post);

            // Conversiones Base64 directas optimizadas con operadores ternarios
            string pdfFile = contenido.archivoPdf != null ? Convert.ToBase64String(contenido.archivoPdf) : null;
            string xmlFile = contenido.archivoXml != null ? Convert.ToBase64String(contenido.archivoXml) : null;

            if (string.IsNullOrEmpty(contenido.token))
            {
                throw new Exception($"El token de correo no existe.");
            }

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-AUTH-TOKEN", contenido.token);

            var payload = new Dictionary<string, object>
            {
                { "from", new { email = contenido.origen, name = "Sistema de Facturación" } },
                { "to", new[] { new { email = contenido.destino, name = "" } } },
                { "subject", contenido.asunto },
                { "attachments", new[]
                    {
                        new { file_name = $"{contenido.nombreArchivo}.pdf", content = pdfFile, content_type = "application/pdf" },
                        new { file_name = $"{contenido.nombreArchivo}.xml", content = xmlFile, content_type = "text/xml" }
                    }
                }
            };

            if (localUsaHtml)
            {
                payload.Add("html_part", cuerpoMensaje);
            }
            else
            {
                payload.Add("text_part", cuerpoMensaje);
                payload.Add("text_part_auto", true);
            }

            request.AddJsonBody(payload);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Error en la API de correo: {response.StatusCode} - {response.Content}");
            }

            return true;
        }

        public async Task<bool> EnviarCorreoGeneralAsync(EmailPayload contenido)
        {
            // Variables locales para evitar colisiones entre hilos asíncronos
            var host = _config.GetSection("EmailSettings:host").Value;
            bool localUsaHtml = contenido.EsHtml;

            // Nota: Lo ideal para alto rendimiento es que '_client' sea inyectado en el constructor de la clase
            var client = new RestClient(host);
            var request = new RestRequest(host, Method.Post);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-AUTH-TOKEN", contenido.token);

            var payload = new Dictionary<string, object>
            {
                { "from", contenido.origen },
                { "to", contenido.destino },
                { "subject", contenido.asunto },
                { "attachments", contenido.attachments }
            };

            if (localUsaHtml)
            {
                // Se verifica si el contenido HTML es válido para el envío por email.
                EmailAnalyzer analisis = new EmailAnalyzer();
                var validacion = analisis.Analyze(contenido.contenido, contenido.tipo);

                if (!validacion.IsSafeToSend) {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("⚠️ EMAIL BLOQUEADO: Riesgo alto de rechazo o diseño roto.");
                    sb.AppendLine($"Puntaje de salud actual: {validacion.Score} / 100 (Mínimo requerido: 90)\n");

                    if (validacion.Errors.Any())
                    {
                        sb.AppendLine("❌ ERRORES CRÍTICOS (Deben corregirse obligatoriamente):");
                        foreach (var error in validacion.Errors)
                        {
                            sb.AppendLine($"  • {error}");
                        }
                        sb.AppendLine(); // Espacio de separación
                    }

                    if (validacion.Warnings.Any())
                    {
                        sb.AppendLine("🔸 ADVERTENCIAS (Recomendado corregir):");
                        foreach (var advertencia in validacion.Warnings)
                        {
                            sb.AppendLine($"  • {advertencia}");
                        }
                    }

                    throw new Exception(sb.ToString());
                }

                payload.Add("html_part", contenido.contenido);
            }
            else
            {
                payload.Add("text_part", contenido.contenido);
                payload.Add("text_part_auto", true);
            }

            request.AddJsonBody(payload);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw new Exception($"Error en la API de correo: {response.StatusCode} - {response.Content}");
            }

            return true;
        }

        // =========================================================================
        // INTERFACES PÚBLICAS OPTIMIZADAS (Reducidas a solo 1 línea de código)
        // =========================================================================

        public async Task<bool> EnviarCorreoAsync(EmailRequest contenido, Models.Documentos.Factura factura)
        {
            // Genera el cuerpo específico e invoca el proceso centralizado
            string body = GenerarContenidoFactura(factura);
            return await EnviarCorreoBaseAsync(contenido, body);
        }

        public async Task<bool> EnviarCorreoAsync(EmailRequest contenido, Models.Documentos.Retencion retencion)
        {
            // Genera el cuerpo específico e invoca el proceso centralizado
            string body = GenerarContenidoRetencion(retencion);
            return await EnviarCorreoBaseAsync(contenido, body);
        }

        public async Task<bool> EnviarCorreoAsync(EmailRequest contenido, Models.Documentos.NotaCredito notacredito)
        {
            // Genera el cuerpo específico e invoca el proceso centralizado
            string body = GenerarContenidoNotaCredito(notacredito);
            return await EnviarCorreoBaseAsync(contenido, body);
        }

        // 1. MÉTODO CENTRALIZADO DE REEMPLAZOS (Evita duplicar las sustituciones comunes)
        private string GenerarContenidoComprobante(
            string tipo,
            dynamic infoTributaria,
            string razonSocialComprador,
            string fechaEmision,
            string valorTotal,
            string dirEstablecimiento)
        {
            // Cargar la plantilla pasando el formato local de forma segura
            string plantilla = definePlantilla(tipo, usaHtml);
            StringBuilder sb = new StringBuilder(plantilla);

            // Interpolación de strings de alto rendimiento para el número de comprobante
            string numeroComprobante = $"{infoTributaria.Estab}-{infoTributaria.PtoEmi}-{infoTributaria.Secuencial}";

            // Marcadores universales idénticos para todos los documentos del SRI
            sb.Replace("{{NOMBRE_CLIENTE}}", razonSocialComprador);
            sb.Replace("{{NUMERO_FACTURA}}", numeroComprobante);
            sb.Replace("{{FECHA_EMISION}}", fechaEmision);
            sb.Replace("{{VALOR_TOTAL}}", valorTotal);
            sb.Replace("{{NOMBRE_EMPRESA}}", infoTributaria.NombreComercial);
            sb.Replace("{{DIRECCION_EMPRESA}}", dirEstablecimiento);
            sb.Replace("{{RUC_EMPRESA}}", infoTributaria.Ruc);

            return sb.ToString();
        }

        // =========================================================================
        // INTERFACES PÚBLICAS REFACTORIZADAS A UNA SOLA LÍNEA DE ENRUTAMIENTO
        // =========================================================================

        public string GenerarContenidoFactura(Models.Documentos.Factura datos)
        {
            return GenerarContenidoComprobante(
                "F", datos.InfoTributaria,
                datos.InfoFactura.RazonSocialComprador,
                datos.InfoFactura.FechaEmision,
                datos.InfoFactura.ImporteTotal,
                datos.InfoFactura.DirEstablecimiento
            );
        }

        public string GenerarContenidoRetencion(Models.Documentos.Retencion datos)
        {
            // CORRECCIÓN DE BUG: Parseo a decimal protegido para evitar crashes por valores con punto/coma decimal
            decimal importeTotal = datos.Impuestos.Impuesto
                .Sum(x => decimal.TryParse(x.Valor, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal v) ? v : 0m);

            return GenerarContenidoComprobante(
                "R", datos.InfoTributaria,
                datos.InfoCompRetencion.RazonSocialSujetoRetenido,
                datos.InfoCompRetencion.FechaEmision,
                importeTotal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), // Formato de moneda limpio
                datos.InfoCompRetencion.DirEstablecimiento
            );
        }

        public string GenerarContenidoNotaCredito(Models.Documentos.NotaCredito datos)
        {
            return GenerarContenidoComprobante(
                "C", datos.InfoTributaria,
                datos.InfoNotaCredito.RazonSocialComprador,
                datos.InfoNotaCredito.FechaEmision,
                datos.InfoNotaCredito.ValorModificacion,
                datos.InfoNotaCredito.DirEstablecimiento
            );
        }

        // =========================================================================
        // MÉTODO DE LECTURA DE PLANTILLAS OPTIMIZADO y THREAD-SAFE
        // =========================================================================
        private string definePlantilla(string tipo, bool usaHtmlFormat)
        {
            // Expresión switch en línea que reemplaza todo el bloque switch procedural largo
            string nombrePlantilla = tipo switch
            {
                "F" => "factura",
                "R" => "retencion",
                "C" => "notacredito",
                "D" => "notadebito",
                _ => throw new ArgumentException("Tipo de comprobante inválido o no soportado.")
            };

            string extension = usaHtmlFormat ? "html" : "txt";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", $"plantilla_{nombrePlantilla}.{extension}");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"No se encontró la plantilla de correo en la ruta: {filePath}");
            }

            return File.ReadAllText(filePath, Encoding.UTF8);
        }
    }
}
