using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TreskelRest.API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, message) = exception switch
            {
                FileNotFoundException => (StatusCodes.Status404NotFound, "El archivo solicitado no existe."),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Usuario o clave Incorrectos."),
                ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado en el servidor.")
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            // 2. Retorna un objeto estándar (ejemplo)
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = exception.GetType().Name, //message,
                Detail = message,
                //Instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
            //throw new NotImplementedException();
        }
    }
}
