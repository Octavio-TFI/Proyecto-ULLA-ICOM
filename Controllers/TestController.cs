using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;
using Controllers.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController(
        ILogger<TestController> _logger,
        IRecibidorMensajes _recibidorMensajes)
        : ControllerBase
    {
        [HttpPost]
        public async Task Post(MensajeTextoPrueba mensaje)
        {
            await _recibidorMensajes.RecibirMensajeTextoAsync(
                new MensajeTextoRecibidoDTO
                {
                    UsuarioId = mensaje.ChatId.ToString(),
                    ChatPlataformaId = mensaje.ChatId.ToString(),
                    Texto = mensaje.Texto,
                    DateTime = mensaje.DateTime,
                    Plataforma = Platforms.Test
                });

            _logger.LogInformation(
                @"
MENSAJE RECIBIDO
Texto: {Texto}
Plataforma: {Plataforma}",
                mensaje.Texto,
                Platforms.Test);
        }
    }
}
