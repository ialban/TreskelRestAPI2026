using AutoMapper;
using Moq;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Models.Correo;
using TreskelRest.Biz.Servicios;
using TreskelRest.Data;
using TreskelRest.Data.Models;

namespace TreskelRest.Biz.Test
{
    public class Tests
    {
        private Mock<ITreskelRepository> _repoMock;
        private Mock<ISendEmail> _sendMailMock;
        private Mock<IMapper> _mapperMock;
        private ProcesarSendMailHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<ITreskelRepository>();
            _sendMailMock = new Mock<ISendEmail>();
            _mapperMock = new Mock<IMapper>();

            _handler = new ProcesarSendMailHandler(
                _repoMock.Object,
                _sendMailMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task EnviaMail()
        {
            string empresaId = "5C107070-422C-4A45-9B4E-8D5565B66D01";
            var emailPayload = new EmailPayload
            {
                origen = new CuentaEmail { email = "info@lacostatech.com", nombre = "LacostaTech"},
                destino = new List<CuentaEmail> { 
                    new CuentaEmail { 
                        email = "ignacio.alban@gmail.com", nombre="Ignacio Albán"
                    } 
                },
                asunto = "Test De Correo Electrónico",
                contenido = "<html>...</html>"
            };

            var command = new ProcesarSendMailCommand(emailPayload, empresaId);

            var empresaSimulada = new UsuarioEmpresa{ tokenEmail = "TOKEN_XYZ_123" };

            _repoMock.Setup(r => r.getEmpresa(empresaId))
                     .Returns(Task.FromResult(empresaSimulada));

            //_sendMailMock.Setup(s => s.EnviarCorreoGeneralAsync(It.IsAny<EmailPayload>()))
            //             .ReturnsAsync(true);

            bool resultado = await _handler.Handle(command, CancellationToken.None);
            Assert.That(resultado, Is.True);

            _repoMock.Verify(r => r.getEmpresa(empresaId), Times.Once);

            _sendMailMock.Verify(s => s.EnviarCorreoGeneralAsync(It.Is<EmailPayload>(p =>
                p.token == "TOKEN_XYZ_123" &&
                p.origen == emailPayload.origen &&
                p.destino == emailPayload.destino &&
                p.asunto == emailPayload.asunto &&
                p.contenido == emailPayload.contenido
            )), Times.Once);

            Assert.Pass();
        }
    }
}
