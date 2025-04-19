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
        IRecibidorMensajes _recibidorMensajes,
        ICalificadorMensajes _calificadorMensajes)
        : ControllerBase
    {
        [HttpPost("texto")]
        public async Task PostMensajeTextoAsync(TestMensajeTexto mensaje)
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

        [HttpGet("calificacion")]
        public async Task PostCalificacionAsync(TestCalificacionMensaje calificacion)
        {
            await _calificadorMensajes.CalificarMensajeAsync(
                new CalificacionMensajeDTO
                {
                    Calificacion = calificacion.Calificacion,
                    MensajePlataformaId = calificacion.MensajeId,
                    Plataforma = Platforms.Test
                });

            _logger.LogInformation(
                @"
CALIFICACION RECIBIDA
Calificación: {Calificacion}
Plataforma: {Plataforma}
MensajePlataformaId: {Id}",
                calificacion.Calificacion,
                Platforms.Test,
                calificacion.MensajeId);
        }
    }
}
