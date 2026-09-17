using System;
using System.Collections.Generic;
using System.Text;

namespace TreskelRest.Biz.Models.Common
{
    public class DocumentoBase
    {
        public byte[] logotipo { get; set; }
        //------------------------------------------------------
        public string empresaid { get; set; }
        public string serial { get; set; }
        public string codigoLocal { get; set; }
        public string nombreLocal { get; set; }
        public string pcName { get; set; }
        public string ipAddress { get; set; }
        public string claveDownload { get; set; }
    }
}
