using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Mappers;
using TreskelRest.Biz.Models;
using TreskelRest.Biz.Models.Common;
using TreskelRest.Biz.Models.Requests;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;
using TreskelRest.Data.Models;

namespace TreskelRest.Biz.Servicios
{
    public record ProcesarComprobanteCommand(Factura factura, Models.Documentos.Factura documento, string empresaId, byte[] xmlfile);

    public class ProcesarComprobanteHandler : ServiciosHandler 
    {
        private readonly ISriComprobanteService _pdfService;
        private readonly ITreskelFilesRepository _files;
        private readonly ISriWebService _sriService;
        private readonly ISendEmail _sendMail;

        public ProcesarComprobanteHandler(
            ISriComprobanteService fileService,
            ITreskelFilesRepository mongoRepo,
            ITreskelRepository repo,
            ISriWebService sriWebService,
            ISendEmail sendMail,
            BizMapperProfile mapper): base(repo, mapper)
        {
            _pdfService = fileService;
            _files = mongoRepo;
            _sriService = sriWebService;
            _sendMail = sendMail;
        }

        public async Task<ComprobanteResponse> Handle(ProcesarComprobanteCommand request, CancellationToken cancellationToken)
        {
            if (request.factura == null)
            {
                throw new ArgumentNullException(nameof(request.factura));
            }

            // 1.- Obtenemos los datos principales de la empresa
            var empresa = await _repo.getEmpresa(request.empresaId);
            if (empresa == null)
            {
                throw new ArgumentNullException(nameof(empresa), $"No se encontró la empresa con ID: {request.empresaId}");
            }

            request.factura.Ambiente = request.documento.InfoTributaria.Ambiente == "1" ? "PRUEBA" : "PRODUCCIÓN";
            request.factura.Autorizacion = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            if (empresa.Logotipo != null)
                request.factura.logotipo = empresa.Logotipo;

            // 2.- Generamos el PDF y XML de la factura
            byte[] pdfBytes = _pdfService.GenerarFacturaPdf(request.factura, request.documento, empresa.Nombre, "", empresa.CreateOnHd);
            byte[] xmlBytes = _pdfService.GenerarFacturaXml(request.factura, request.documento, empresa.Nombre, "", "AUTORIZADO", empresa.CreateOnHd, request.xmlfile);

            // 3.- Se registran los archivos en MongoDB
            string fileId = "";
            if (pdfBytes != null && xmlBytes != null && empresa.UsaMongoDB)
            {
                MongoRequest mongoRequest = new MongoRequest
                {
                    claveSri = request.factura.ClaveAcceso,
                    empresaId = request.empresaId,
                    dataFile = new MongoDataFile
                    {
                        nombreArchivo = request.factura.ClaveAcceso,
                        tipoArchivoPdf = "application/pdf",
                        archivoPdf = pdfBytes,
                        tipoArchivoXml = "application/xml",
                        archivoXml = xmlBytes
                    }
                };

                fileId = await _files.SubirArchivoAsync(mongoRequest);
            }

            // 4.- Se registra la factura en la base de datos

            Facturacion _factura = _mapper.FacturaToDto(request.factura);
            if (pdfBytes != null)
                _factura.pdfGenerado = true;

            var proceso_grabado = await _repo.grabarFactura(_factura, fileId);
            _mapper.UpdateFacturacion(proceso_grabado, _factura);

            // 5.- Se envía la factura al SRI para su validación
            RespuestaSRI sriRespuesta = await _sriService.validaDocumento(
                request.factura.Ambiente,
                request.xmlfile,
                request.factura.ClaveAcceso);

            // 7.- Se envía los archivos vía email al cliente 
            _factura.estadoEnvioMail = "N";
            _factura.enviadoCliente = false;

            EmailRequest contenido = new EmailRequest
            {
                token = empresa.tokenEmail,
                origen = empresa.email,
                destino = _factura.emailCliente,
                archivoPdf = pdfBytes,
                archivoXml = xmlBytes,
                asunto = "Comprobante Electrónico de Facturación",
                contenido = $"",
                nombreArchivo = _factura.claveAcceso
            };

            var correo = await _sendMail.EnviarCorreoAsync(contenido, request.documento);

            // 6.- Se actualiza la data en la base de datos
            _factura.estadoCodigo = DecodificaEstadosSRI(sriRespuesta.Estado);
            if (sriRespuesta.Estado == "AUTORIZADO")
            {
                _factura.fechaAutorizacion = DateTime.Now;
                _factura.mensaje = sriRespuesta.Mensaje;
                _factura.tipo = sriRespuesta.Tipo;
            }

            proceso_grabado = await _repo.actualizaFactura(_factura);
            _mapper.UpdateFacturacion(proceso_grabado, _factura);

            // 7.- Se retorna el resultado
            ComprobanteResponse comprobante = new ComprobanteResponse
            {
                Tipo = "F",
                ClaveAcceso = _factura.claveAcceso,
                Estado = _factura.estadoCodigo,
                EstadoMail = (bool) _factura.enviadoCliente,
                Mensaje = _factura.mensaje,
                archivos = null
            };

            return comprobante;
        }
    }
}
