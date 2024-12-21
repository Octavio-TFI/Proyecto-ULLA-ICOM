using AppServices.Abstractions.DTOs;

namespace AppServices.Abstractions
{
    /// <summary>
    /// Interfaz para recibir mensajes
    /// </summary>
    public interface IRecibidorMensajes
    {
        /// <summary>
        /// Recibe un mensaje de texto y lo procesa
        /// </summary>
        /// <param name="mensaje">Mensaje a recibir</param>
        Task RecibirMensajeTextoAsync(MensajeTextoRecibidoDTO mensaje);
    }
}
