using AutoMapper;
using MediatR;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Models;
using TreskelRest.Biz.Models.Requests;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;
using TreskelRest.Data.Base;

namespace TreskelRest.Biz.Servicios
{
    public record ProcesarRetencionCommand(Retencion retencion, Models.Documentos.Retencion documento, string empresaId, byte[] xmlfile) : IRequest<ComprobanteResponse>;

    public class ProcesarRetencionHandler : ProcessHandler, IRequestHandler<ProcesarRetencionCommand, ComprobanteResponse>
    {
        private readonly ISriComprobanteService _pdfService;
        private readonly ITreskelFilesRepository _files;
        private readonly ITreskelRepository _repo;
        private readonly ISriWebService _sriService;
        private readonly ISendEmail _sendMail;
        private readonly IMapper _mapper;

        public ProcesarRetencionHandler(
            ISriComprobanteService pdfService,
            ITreskelFilesRepository mongoRepo,
            ITreskelRepository repo,
            ISriWebService sriWebService,
            ISendEmail sendMail,
            IMapper mapper)
        {
            _pdfService = pdfService;
            _files = mongoRepo;
            _repo = repo;
            _sriService = sriWebService;
            _sendMail = sendMail;
            _mapper = mapper;
        }

        public async Task<ComprobanteResponse> Handle(ProcesarRetencionCommand request, CancellationToken cancellationToken)
        {
            if (request.retencion == null)
            {
                throw new ArgumentNullException(nameof(request.retencion));
            }

            // 1.- Obtenemos los datos principales de la empresa
            var empresa = await _repo.getEmpresa(request.empresaId);
            if (empresa == null)
            {
                throw new ArgumentNullException(nameof(empresa), $"No se encontró la empresa con ID: {request.empresaId}");
            }

            request.retencion.Ambiente = request.documento.InfoTributaria.Ambiente == "1" ? "PRUEBA" : "PRODUCCIÓN";
            request.retencion.Autorizacion = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            if (empresa.Logotipo != null)
                request.retencion.logotipo = empresa.Logotipo;

            // 2.- Generamos el PDF de la factura
            byte[] pdfBytes = _pdfService.GenerarRetencionPdf(request.retencion, request.documento, empresa.Nombre, "", empresa.CreateOnHd);

            // 3.- Se registran los archivos en MongoDB
            string fileId = "";
            if (pdfBytes != null && request.xmlfile != null && empresa.UsaMongoDB)
            {
                Data.Models.MongoRequest mongoRequest = new Data.Models.MongoRequest
                {
                    claveSri = request.retencion.ClaveAcceso,
                    empresaId = request.empresaId,
                    dataFile = new Data.Models.MongoDataFile
                    {
                        nombreArchivo = request.retencion.ClaveAcceso,
                        tipoArchivoPdf = "application/pdf",
                        archivoPdf = pdfBytes,
                        tipoArchivoXml = "application/xml",
                        archivoXml = request.xmlfile
                    }
                };

                fileId = await _files.SubirArchivoAsync(mongoRequest);
            }

            // 4.- Se registra la factura en la base de datos

            Data.Models.Retencion _retencion = _mapper.Map<Data.Models.Retencion>(request.retencion);
            if (pdfBytes != null)
                _retencion.pdfGenerado = true;

            _retencion.empresaid = request.empresaId;
            _retencion.serial = request.retencion.Secuencial;
            _retencion.codigoLocal = request.retencion.codigoLocal;
            _retencion.idCliente = request.documento.InfoCompRetencion.IdentificacionSujetoRetenido;
            _retencion.nombreCliente = request.documento.InfoCompRetencion.RazonSocialSujetoRetenido;
            _retencion.emailCliente = request.documento.InfoAdicional.CampoAdicional.Where(x => x.Nombre == "Email").Select(y => y.Text).FirstOrDefault();

            var proceso_grabado = await _repo.grabarRetencion(_retencion, fileId);
            _mapper.Map(proceso_grabado, _retencion);

            // 5.- Se envía la factura al SRI para su validación
            RespuestaSRI sriRespuesta = await _sriService.validaDocumento(
                request.retencion.Ambiente,
                request.xmlfile,
                request.retencion.ClaveAcceso);

            if (sriRespuesta.Estado == "AUTORIZADO")
            {
                // 7.- Se envía los archivos vía email al cliente 
                _retencion.estadoEnvioMail = "N";
                _retencion.enviadoCliente = false;

                EmailRequest contenido = new EmailRequest
                {
                    token = empresa.tokenEmail,
                    origen = empresa.email,
                    destino = _retencion.emailCliente,
                    archivoPdf = pdfBytes,
                    archivoXml = request.xmlfile,
                    asunto = "Comprobante Electrónico de Retención",
                    contenido = $"",
                    nombreArchivo = _retencion.claveAcceso
                };

                var correo = await _sendMail.EnviarCorreoAsync(contenido, request.documento);
            }

            // 6.- Se actualiza la data en la base de datos
            _retencion.estadoCodigo = DecodificaEstadosSRI(sriRespuesta.Estado);
            if (sriRespuesta.Estado == "AUTORIZADO")
            {
                _retencion.fechaAutorizacion = DateTime.Now;
                _retencion.mensaje = sriRespuesta.Mensaje;
                _retencion.tipo = sriRespuesta.Tipo;
            }

            proceso_grabado = await _repo.actualizaRetencion(_retencion);
            _mapper.Map(proceso_grabado, _retencion);

            // 7.- Se retorna el resultado
            ComprobanteResponse comprobante = new ComprobanteResponse
            {
                Tipo = "R",
                ClaveAcceso = _retencion.claveAcceso,
                Estado = _retencion.estadoCodigo,
                EstadoMail = _retencion.enviadoCliente ?? false,
                Mensaje = _retencion.mensaje,
                archivos = null
            };

            return comprobante;
        }
    }
}
