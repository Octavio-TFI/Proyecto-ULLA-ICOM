using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController(ILogger<TestController> _logger)
        : ControllerBase
    {
        [HttpPost]
        public Task Post(MensajePrueba mensajePrueba)
        {
            _logger.LogInformation("Mensaje recibido: {Texto}", mensajePrueba.Texto);

            return Task.CompletedTask;
        }
    }
}
