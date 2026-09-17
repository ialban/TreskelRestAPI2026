using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TreskelRest.Biz.Models;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Data;
using TreskelRest.Data.Models;

namespace TreskelRest.Biz.Servicios
{
    public record DownloadDocummentCommand(Factura factura, string tipo) : IRequest<ArchivoResponse>;
    public class DownloadDocumentsHandler : IRequestHandler<DownloadDocummentCommand, ArchivoResponse>
    {
        private readonly ITreskelFilesRepository _files;
        private readonly ITreskelRepository _repo;
        private readonly IMapper _mapper;

        public DownloadDocumentsHandler(
            ITreskelFilesRepository mongoRepo,
            ITreskelRepository repo,
            IMapper mapper)
        {
            _files = mongoRepo;
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ArchivoResponse> Handle(DownloadDocummentCommand request, CancellationToken cancellationToken)
        {
            Facturacion _factura = _mapper.Map<Facturacion>(request.factura);

            // 1.- Obtener el path del archivo a descargar o el archivoid de mongo
            var fileInfo = await _repo.obtenerFactura(_factura, request.tipo.Substring(0,1).ToUpper());

            if (fileInfo == null)
            {
                throw new FileNotFoundException("Archivo no encontrado");
            }

            // 2.- Si tiene archivoid, obtengo la data desde mongodb
            //     Si no obtengo el dato path del archivo (retorna string)
            if (string.IsNullOrEmpty(fileInfo.archivoPath) || !string.IsNullOrEmpty(fileInfo.archivoId)){
                var fileResponse = await _files.DescargarArchivoAsync(fileInfo.archivoId);

                var response = new ArchivoResponse { 
                    Contenido = fileResponse.ArchivoPdf,
                    NombreArchivo = fileResponse.NombreArchivoPdf,
                    ContentType = fileResponse.TipoArchivoPdf
                };

                return response;
            }
            else
            {
                string filename = string.Empty;
                var tipo = request.tipo.Substring(0, 1).ToUpper();

                string prefijo = tipo.ToUpper() switch
                {
                    "F" => "",
                    "R" => "R_",
                    "C" => "NC_",
                    "D" => "ND_",
                    _ => throw new Exception("Tipo inválido")
                };

                string dirBase = AppContext.BaseDirectory;

                filename = Path.Combine(dirBase, "PDFs", fileInfo.archivoPath,$"{prefijo}{request.factura.ClaveAcceso}.pdf");

                var response = new ArchivoResponse
                {
                    Contenido = File.ReadAllBytes(filename),
                    NombreArchivo = Path.GetFileName(filename),
                    ContentType = "application/pdf"
                };

                return response;
            }
            
            throw new NotImplementedException();
        }
    }
}
