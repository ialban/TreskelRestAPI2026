using System;
using System.Collections.Generic;
using System.Text;

namespace TreskelRest.Biz.Interfaces
{
    public interface IWhatsapp
    {
        Task<bool> EnviarMensajeAsync(Biz.Models.Correo.MensajeWs contenido, string token);
    }
}
