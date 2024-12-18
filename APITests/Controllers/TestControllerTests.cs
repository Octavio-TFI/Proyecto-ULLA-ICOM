using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;

namespace API.Controllers.Tests
{
    [TestFixture()]
    public class TestControllerTests
    {
        [Test()]
        public void PostTest()
        {
            // Arrange
            var mensaje = new MensajeTextoPrueba
            {
                ChatId = new Guid(),
                Texto = "Test",
                DateTime = DateTime.Now
            };

            var loggerMock = new Mock<ILogger<TestController>>();
            var recibidorMensajesMock = new Mock<IRecibidorMensajes>();

            var testController = new TestController(
                loggerMock.Object,
                recibidorMensajesMock.Object);

            // Act
            var result = testController.Post(mensaje);

            // Assert
            loggerMock.VerifyLog()
                .InformationWasCalled()
                .MessageEquals("Mensaje recibido: Test")
                .Times(1);

            recibidorMensajesMock.Verify(
                x => x.RecibirMensajeTextoAsync(
                    It.Is<MensajeTextoDTO>(
                        m => m.UsuarioId == mensaje.ChatId.ToString() &&
                            m.ChatId == mensaje.ChatId.ToString() &&
                            m.Texto == mensaje.Texto &&
                            m.Plataforma == "Test" &&
                            m.DateTime == mensaje.DateTime)),
                Times.Once);

            Assert.That(result, Is.EqualTo(Task.CompletedTask));
        }
    }
}