using System;
using System.Collections.Generic;
using System.Text;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Models.Correo;

namespace TreskelRest.Biz.Models.Responses
{
    public class Whatsapp : IWhatsapp
    {
        public Task<bool> EnviarMensajeAsync(MensajeWs contenido, string token)
        {


            throw new NotImplementedException();
        }
    }
}
