using Microsoft.AspNetCore.Mvc;
using TreskelRest.API.Common;
using TreskelRest.API.Mappers;
using TreskelRest.Biz.Models.Responses;
using TreskelRest.Biz.Servicios;
using Wolverine;

namespace TreskelRest.API.Controllers
{
    [Route("[controller]")]
    public class DocumentosController : TreskelController
    {
        public DocumentosController(IConfiguration config, IMessageBus mediator, APIMapperProfile mapper) : base(config, mediator, mapper)
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
                var factura = _mapper.ToBizFactura(payload);

                var response = await _mediator.InvokeAsync<ArchivoResponse>(new DownloadDocummentCommand(factura, tipo));

                respuesta.EsError = false;

                return File(response.Contenido, response.ContentType, response.NombreArchivo);
            }

            return Unauthorized(respuesta);
        }
    }
}
