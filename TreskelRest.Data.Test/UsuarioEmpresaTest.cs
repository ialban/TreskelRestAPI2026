using TreskelRest.Data.Base;
using TreskelRest.Data.Models;
using TreskelRest.Data.Test.Base;

namespace TreskelRest.Data.Test
{
    public class UsuarioEmpresaTest: UnitTestBase
    {
        [Test]
        public async Task Login()
        {
            UsuarioRequest payload = new UsuarioRequest
            {
                Usuario = "rcaosa",
                Ruc = "0992715405001",
                EmpresaId = "2C27EEE2-04D4-4F63-84AD-93535046D040"
            };

            UsuarioEmpresa resultado = await _repo.login(payload);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.Nombre, Is.EqualTo("Dinastía Sur"));
            Assert.That(resultado.Activo, Is.EqualTo(true));
        }

        [Test]
        public async Task ActualizaCliente()
        {
            UsuarioEmpresa payload = new UsuarioEmpresa
            {
                Ruc = "0992715405001",
                EmpresaId = "2C27EEE2-04D4-4F63-84AD-93535046D040",
                Logotipo = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 }
            };

            UsuarioEmpresa resultado = await _repo.actualizaCliente(payload);

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.Ruc, Is.EqualTo("0992715405001"));
        }
    }
}
