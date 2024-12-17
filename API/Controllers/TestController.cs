using API.DTOs;
using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController(
        ILogger<TestController> _logger,
        IRecibidorMensajes _recibidorMensajes)
        : ControllerBase
    {
        [HttpPost]
        public async Task Post(MensajePrueba mensaje)
        {
            await _recibidorMensajes.RecibirMensajeAsync(
                new MensajeDTO
                {
                    UsuarioId = mensaje.ChatId.ToString(),
                    ChatId = mensaje.ChatId.ToString(),
                    Texto = mensaje.Texto,
                    DateTime = mensaje.DateTime,
                    Plataforma = "Test",
                    TipoMensaje = TipoMensajeDTO.Texto
                });

            _logger.LogInformation("Mensaje recibido: {Texto}", mensaje.Texto);
        }
    }
}
