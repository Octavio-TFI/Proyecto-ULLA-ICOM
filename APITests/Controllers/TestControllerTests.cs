namespace API.Controllers.Tests
{
    [TestFixture()]
    public class TestControllerTests
    {
        [Test()]
        public void PostTest()
        {
            // Arrange
            var mensaje = new MensajePrueba { ChatId = Guid.Empty, Texto = "Test" };

            var loggerMock = new Mock<ILogger<TestController>>();

            var testController = new TestController(loggerMock.Object);

            // Act
            var result = testController.Post(mensaje);

            // Assert
            loggerMock.VerifyLog()
                .InformationWasCalled()
                .MessageEquals("Mensaje recibido: Test")
                .Times(1);

            Assert.That(result, Is.EqualTo(Task.CompletedTask));
        }
    }
}