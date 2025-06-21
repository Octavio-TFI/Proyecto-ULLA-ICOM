using AppServices.Abstractions.DTOs;
using Domain.Entities.ChatAgregado;
using Moq;

namespace AppServices.Tests
{
    [TestFixture()]
    public class CalificadorMensajesTests
    {
        [Test()]
        [TestCase(true)]
        [TestCase(false)]
        public async Task CalificarMensajeAsync_ValidMensajeIATest(
            bool calificacion)
        {
            // Arrange
            var calificacionMensajeDTO = new CalificacionMensajeDTO
            {
                MensajePlataformaId = "mensaje1",
                Plataforma = "Test",
                Calificacion = calificacion
            };

            var mensajeIA = new MensajeIA
            {
                Id = Guid.NewGuid(),
                DateTime = DateTime.Now,
                Texto = "texto"
            };

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var mensajeIARepositoryMock = new Mock<IMensajeIARepository>();

            mensajeIARepositoryMock.Setup(
                r => r.GetAsync(
                    calificacionMensajeDTO.MensajePlataformaId,
                    calificacionMensajeDTO.Plataforma))
                .ReturnsAsync(mensajeIA);

            var calificadorMensajes = new CalificadorMensajes(
                unitOfWorkMock.Object,
                mensajeIARepositoryMock.Object);

            // Act
            await calificadorMensajes.CalificarMensajeAsync(
                calificacionMensajeDTO);

            // Assert
            Assert.That(
                mensajeIA.Calificacion,
                Is.EqualTo(calificacionMensajeDTO.Calificacion));

            unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
