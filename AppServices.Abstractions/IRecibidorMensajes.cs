using AppServices.Abstractions.DTOs;

namespace AppServices.Abstractions
{
    /// <summary>
    /// Interfaz para recibir mensajes
    /// </summary>
    public interface IRecibidorMensajes
    {
        /// <summary>
        /// Recibe un mensaje y lo procesa
        /// </summary>
        /// <param name="mensaje">Mensaje a recibir</param>
        Task RecibirMensajeAsync(MensajeDTO mensaje);
    }
}
