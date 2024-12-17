using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController(ILogger<TestController> _logger)
        : ControllerBase
    {
        [HttpPost]
        public Task Post()
        {
            return Task.CompletedTask;
        }
    }
}
