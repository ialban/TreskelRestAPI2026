using System;
using System.Collections.Generic;
using System.Text;

namespace TreskelRest.Biz.Models.Correo
{
    public class MensajeWs
    {
        public List<string> destino { get; set; }
        public string titulo { get; set; }
        public string mensaje { get; set; }
    }
}
