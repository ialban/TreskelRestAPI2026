using HtmlAgilityPack;
using System.Text;
using TreskelRest.Biz.Extensions;

namespace TreskelRest.Biz.Models.Correo
{
    public class EmailAnalyzer
    {
        private static readonly string[] SpamTriggerWords =
        {
            "gratis", "ganador", "dinero", "compre ahora", "100% libre",
            "urgente", "millones", "inversión", "felicidades", "oferta exclusiva",
            "ingresos extra", "sin costo", "actúe rápido", "garantizado", "ingresos adicionales",
            "dinero fácil", "multiplica tus ganancias", "retornos garantizados", "elimina tus deudas",
            "tarjeta de crédito preaprobada", "reembolso completo", "ganancia neta", "gana diniero desde casa",
            "hazlo ya", "vence hoy", "tiempo limitado", "última oportunidad", "inmediato", "apúrate", 
            "no esperes más", "solo por 24 horas", "termina pronto", "¡atención!",
            "gran rebaja", "descuento masivo", "mejor precio", "ahorra dinero", "regalo promocional",
            "liquidación total", "oferta de por vida", "paga menos",
            "verifica tu cuenta", "acción requerida", "alerta de seguridad", "inicio de sesión sospechoso",
            "actualiza tus datos", "pago rechazado", "cuenta suspendida"
        };

        public EmailAnalysisResult Analyze(string content, EmailType tipo)
        {
            var result = new EmailAnalysisResult();

            if (string.IsNullOrEmpty(content))
            {
                result.Errors.Add("El contenido está vacío");
                result.Score = 0;
                return result;
            }

            result.FileSizeInBytes = Encoding.UTF8.GetByteCount(content);
            double fileSizeInKBytes = result.FileSizeInBytes / 1024.0;

            if (fileSizeInKBytes > 90)
            {
                result.Errors.Add($"El tamaño del contenido {fileSizeInKBytes:0.0} KB es muy alto, supera el umbral de 90 KB.");
                result.Score -= 20;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var tables = doc.DocumentNode.SelectNodes("//table");
            if (tables == null || !tables.Any())
            {
                result.Errors.Add("Estructura inválida. Debe hacer uso de etiquetas <table>");
                result.Score -= 20;
            }

            var styleNodes = doc.DocumentNode.SelectNodes("//style");
            if (styleNodes != null && styleNodes.Any())
            {
                result.Warnings.Add("Se encontraron bloques <style>. Los firewalls corporativos los bloquean por seguridad. Usa exclusivamente CSS inline.");
                result.Score -= 25;
            }

            var images = doc.DocumentNode.SelectNodes("//img");
            if (images != null)
            {
                foreach (var img in images)
                {
                    string src = img.GetAttributeValue("src", "");
                    string alt = img.GetAttributeValue("alt", "").Trim();

                    if (string.IsNullOrEmpty(alt))
                    {
                        result.Errors.Add($"Imagen sin atributo 'alt' (src='{src}'). Los filtros estrictos marcan como spam las imágenes ocultas sin descripción.");
                        result.Score -= 15;
                    }

                    if (src.StartsWith("/") || src.StartsWith("file://"))
                    {
                        result.Errors.Add($"Ruta de imagen local/relativa no permitida: '{src}'.");
                        result.Score -= 30;
                    }
                }
            }

            var links = doc.DocumentNode.SelectNodes("//a");
            bool tieneEnlaceDesuscripcion = false;

            if (links != null)
            {
                foreach (var link in links)
                {
                    string href = link.GetAttributeValue("href", "").Trim();
                    string textoEnlace = link.InnerText.ToLower();

                    // Validar si incluye términos de desuscripción obligatorios por ley
                    if (textoEnlace.Contains("cancelar") || textoEnlace.Contains("dessuscribir") ||
                        textoEnlace.Contains("unsubscribe") || textoEnlace.Contains("salir de la lista"))
                    {
                        tieneEnlaceDesuscripcion = true;
                    }

                    if (string.IsNullOrEmpty(href) || href == "#")
                    {
                        result.Errors.Add("Se detectó un enlace vacío o roto apuntando a '#'.");
                        result.Score -= 15;
                    }
                    else if (href.StartsWith("http://"))
                    {
                        result.Warnings.Add($"Enlace inseguro detectado: '{href}'. Utiliza siempre HTTPS para firewalls corporativos.");
                        result.Score -= 10;
                    }
                    else if (!href.StartsWith("https://") && !href.StartsWith("mailto:"))
                    {
                        result.Errors.Add($"El enlace '{href}' no utiliza un protocolo absoluto válido.");
                        result.Score -= 20;
                    }
                }
            }

            if (tipo.RequiereUnsubscribe() && !tieneEnlaceDesuscripcion)
            {
                result.Errors.Add("Cumplimiento legal fallido: No se detectó un enlace de desuscripción ('cancelar suscripción', 'unsubscribe'). Los servidores más estrictos rebotarán el correo de inmediato.");
                result.Score -= 30;
            }

            // 7. Filtro de palabras Spam (Penalización incrementada)
            string plainText = doc.DocumentNode.InnerText.ToLower();
            foreach (var word in SpamTriggerWords)
            {
                if (plainText.Contains(word))
                {
                    result.Warnings.Add($"Palabra de alto riesgo corporativo detectada: '{word}'.");
                    result.Score -= 15; // Penaliza más fuerte por palabra
                }
            }

            result.Score = Math.Max(0, result.Score);
            return result;
        }
    }
}
