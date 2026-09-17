using System;
using System.Collections.Generic;
using System.Text;

namespace TreskelRest.Biz.Models.Correo
{
    public class EmailAnalysisResult
    {
        public int Score { get; set; } = 100;
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
        public long FileSizeInBytes { get; set; }

        // CALIBRACIÓN CORPORATIVA: Sin errores y un puntaje estricto de 90 o más
        public bool IsSafeToSend => Errors.Count == 0 && Score >= 90;
    }
}
