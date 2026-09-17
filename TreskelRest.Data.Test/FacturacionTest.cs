using System;
using System.Collections.Generic;
using System.Text;
using TreskelRest.Data.Base;
using TreskelRest.Data.Models;
using TreskelRest.Data.Test.Base;

namespace TreskelRest.Data.Test
{
    public class FacturacionTest : UnitTestBase
    {
        [Test]
        public async Task getEmpresa()
        {
            string EmpresaId = "2C27EEE2-04D4-4F63-84AD-93535046D040";

            UsuarioEmpresa resultado = await _repo.getEmpresa(EmpresaId);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.email, Is.EqualTo("restrcao@lacostatech.com"));
            Assert.That(resultado.Nombre.ToUpper(), Is.EqualTo("DINASTÍA SUR"));
            Assert.That(resultado.Activo, Is.EqualTo(true));
        }

        [Test]
        public async Task verificaFactura()
        {
            Facturacion payload = new Facturacion
            {
                empresaid = "2C27EEE2-04D4-4F63-84AD-93535046D040",
                secuencial = "0000043568",
                claveAcceso = "0507202301099271540500120010020000229060000004416"
            };

            Facturacion resultado = await _repo.obtenerFactura(payload, "F");

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.secuencial, Is.EqualTo("0000043568"));
            Assert.That(resultado.serial, Is.EqualTo("000022906"));
        }
    }
}
