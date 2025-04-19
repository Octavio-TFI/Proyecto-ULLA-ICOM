using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;
using Controllers;
using Controllers.DTOs;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace API.Controllers.Tests
{
    [TestFixture()]
    public class TestControllerTests
    {
        [Test()]
        public void PostMensajeTextoAsync()
        {
            // Arrange
            var mensaje = new TestMensajeTexto
            {
                ChatId = new Guid(),
                Texto = "Test",
                DateTime = DateTime.Now
            };

            var loggerMock = new Mock<ILogger<TestController>>();
            var recibidorMensajesMock = new Mock<IRecibidorMensajes>();
            var calificadorMensajesMock = new Mock<ICalificadorMensajes>();

            var testController = new TestController(
                loggerMock.Object,
                recibidorMensajesMock.Object,
                calificadorMensajesMock.Object);

            // Act
            var result = testController.PostMensajeTextoAsync(mensaje);

            // Assert
            loggerMock.VerifyLog()
                .InformationWasCalled()
                .MessageEquals(
                    @"
MENSAJE RECIBIDO
Texto: Test
Plataforma: Test")
                .Times(1);

            recibidorMensajesMock.Verify(
                x => x.RecibirMensajeTextoAsync(
                    It.Is<MensajeTextoRecibidoDTO>(
                        m => m.UsuarioId == mensaje.ChatId.ToString() &&
                            m.ChatPlataformaId == mensaje.ChatId.ToString() &&
                            m.Texto == mensaje.Texto &&
                            m.Plataforma == "Test" &&
                            m.DateTime == mensaje.DateTime)),
                Times.Once);

            Assert.That(result, Is.EqualTo(Task.CompletedTask));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task PostCalificacionAsync(bool calificacion)
        {
            // Arrange
            var calificacionMensaje = new TestCalificacionMensaje
            {
                MensajeId = "test-message-id",
                Calificacion = calificacion
            };

            var loggerMock = new Mock<ILogger<TestController>>();
            var recibidorMensajesMock = new Mock<IRecibidorMensajes>();
            var calificadorMensajesMock = new Mock<ICalificadorMensajes>();

            var testController = new TestController(
                loggerMock.Object,
                recibidorMensajesMock.Object,
                calificadorMensajesMock.Object);

            // Act
            await testController.PostCalificacionAsync(calificacionMensaje);

            // Assert
            loggerMock.VerifyLog()
                .InformationWasCalled()
                .MessageEquals(
                    $@"
CALIFICACION RECIBIDA
Calificación: {calificacion}
Plataforma: Test
MensajePlataformaId: test-message-id")
                .Times(1);

            calificadorMensajesMock.Verify(
                x => x.CalificarMensajeAsync(
                    It.Is<CalificacionMensajeDTO>(
                        m => m.MensajePlataformaId ==
                            calificacionMensaje.MensajeId &&
                            m.Calificacion == calificacionMensaje.Calificacion &&
                            m.Plataforma == "Test")),
                Times.Once);
        }
    }
}