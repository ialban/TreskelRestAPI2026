using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Common;
using TreskelRest.Biz.Servicios;

namespace TreskelRest.API.Controllers
{
    [Route("[controller]")]
    public class DocumentosController : TreskelController
    {
        public DocumentosController(IConfiguration config, IMediator mediator, IMapper mapper) : base(config, mediator, mapper)
        {

        }

        [HttpPost("descargar/{tipo}")]
        public async Task<ActionResult> download(FacturaModel payload, string token, string tipo)
        {
            var valido = ValidateToken(token);
            var respuesta = new RespuestaError { 
                EsError = true,
                Mensaje = "Usuario o clave no autorizado",
                StatusCode = 401
            };

            if (valido != null) {
                string filename = string.Empty;

                // Obtenemos el archivo a descargar según el tipo de documento
                var factura = _mapper.Map<Biz.Models.Factura>(payload);
                var response = await _mediator.Send(new DownloadDocummentCommand(factura, tipo));

                respuesta.EsError = false;

                return File(response.Contenido, response.ContentType, response.NombreArchivo);
            }

            return Unauthorized(respuesta);
        }
    }
}
