using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Mappers;
using TreskelRest.Biz.Models;
using TreskelRest.Biz.Models.Common;
using TreskelRest.Biz.Models.Requests;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;

namespace TreskelRest.Biz.Servicios
{
    public record ProcesarNotaCreditoCommand(NotaCredito notacredito, Models.Documentos.NotaCredito documento, string empresaId, byte[] xmlfile);
    public class ProcesarNotaCreditoHandler : ServiciosHandler
    {
        private readonly ISriComprobanteService _pdfService;
        private readonly ITreskelFilesRepository _files;
        private readonly ISriWebService _sriService;
        private readonly ISendEmail _sendMail;

        public ProcesarNotaCreditoHandler(
            ISriComprobanteService pdfService,
            ITreskelFilesRepository mongoRepo,
            ITreskelRepository repo,
            ISriWebService sriWebService,
            ISendEmail sendMail,
            BizMapperProfile mapper): base(repo, mapper)
        {
            _pdfService = pdfService;
            _files = mongoRepo;
            _sriService = sriWebService;
            _sendMail = sendMail;
        }

        public async Task<ComprobanteResponse> Handle(ProcesarNotaCreditoCommand request, CancellationToken cancellationToken)
        {
            if (request.notacredito == null)
            {
                throw new ArgumentNullException(nameof(request.notacredito));
            }

            // 1.- Obtenemos los datos principales de la empresa
            var empresa = await _repo.getEmpresa(request.empresaId);
            if (empresa == null)
            {
                throw new ArgumentNullException(nameof(empresa), $"No se encontró la empresa con ID: {request.empresaId}");
            }

            request.notacredito.Ambiente = request.documento.InfoTributaria.Ambiente == "1" ? "PRUEBA" : "PRODUCCIÓN";
            request.notacredito.Autorizacion = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            if (empresa.Logotipo != null)
                request.notacredito.logotipo = empresa.Logotipo;

            // 2.- Generamos el PDF de la factura
            byte[] pdfBytes = _pdfService.GenerarNotaCreditoPdf(request.notacredito, request.documento, empresa.Nombre, "", empresa.CreateOnHd);
            byte[] xmlBytes = _pdfService.GenerarNotaCreditoXml(request.notacredito, request.documento, empresa.Nombre, "", "AUTORIZADO", empresa.CreateOnHd);

            // 3.- Se registran los archivos en MongoDB
            string fileId = "";
            if (pdfBytes != null && xmlBytes != null && empresa.UsaMongoDB)
            {
                Data.Models.MongoRequest mongoRequest = new Data.Models.MongoRequest
                {
                    claveSri = request.notacredito.ClaveAcceso,
                    empresaId = request.empresaId,
                    dataFile = new Data.Models.MongoDataFile
                    {
                        nombreArchivo = request.notacredito.ClaveAcceso,
                        tipoArchivoPdf = "application/pdf",
                        archivoPdf = pdfBytes,
                        tipoArchivoXml = "application/xml",
                        archivoXml = xmlBytes
                    }
                };

                fileId = await _files.SubirArchivoAsync(mongoRequest);
            }

            // 4.- Se registra la factura en la base de datos

            Data.Models.NotaCredito _notacredito = _mapper.NotaCreditoToDto(request.notacredito);
                
            if (pdfBytes != null)
                _notacredito.pdfGenerado = true;

            var proceso_grabado = await _repo.grabarNotaCredito(_notacredito, fileId);
            _mapper.UpdateNotaCredito(proceso_grabado, _notacredito);

            // 5.- Se envía la factura al SRI para su validación
            RespuestaSRI sriRespuesta = await _sriService.validaDocumento(
                request.notacredito.Ambiente,
                request.xmlfile,
                request.notacredito.ClaveAcceso);

            // 7.- Se envía los archivos vía email al cliente 
            _notacredito.estadoEnvioMail = "N";
            _notacredito.enviadoCliente = false;

            EmailRequest contenido = new EmailRequest
            {
                token = empresa.tokenEmail,
                origen = empresa.email,
                destino = _notacredito.emailCliente,
                archivoPdf = pdfBytes,
                archivoXml = xmlBytes,
                asunto = "Comprobante Electrónico de Nota de Crédito",
                contenido = $"",
                nombreArchivo = _notacredito.claveAcceso
            };

            var correo = await _sendMail.EnviarCorreoAsync(contenido, request.documento);

            // 6.- Se actualiza la data en la base de datos
            _notacredito.estadoCodigo = DecodificaEstadosSRI(sriRespuesta.Estado);
            if (sriRespuesta.Estado == "AUTORIZADO")
            {
                _notacredito.fechaAutorizacion = DateTime.Now;
                _notacredito.mensaje = sriRespuesta.Mensaje;
                _notacredito.tipo = sriRespuesta.Tipo;
            }

            proceso_grabado = await _repo.actualizaNotaCredito(_notacredito);
            _mapper.UpdateNotaCredito(proceso_grabado, _notacredito);

            // 7.- Se retorna el resultado
            ComprobanteResponse comprobante = new ComprobanteResponse
            {
                Tipo = "C",
                ClaveAcceso = _notacredito.claveAcceso,
                Estado = _notacredito.estadoCodigo,
                EstadoMail = (bool)_notacredito.enviadoCliente,
                Mensaje = _notacredito.mensaje,
                archivos = null
            };

            return comprobante;
        }
    }
}
