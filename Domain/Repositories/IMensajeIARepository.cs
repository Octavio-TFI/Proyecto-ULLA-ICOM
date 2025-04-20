using Domain.Entities.ChatAgregado;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IMensajeIARepository
    {
        /// <summary>
        /// Obtiene un mensaje IA por su ID.
        /// </summary>
        /// <param name="id">ID del mensaje IA</param>
        /// <returns>MensajeIA con ese ID</returns>
        Task<MensajeIA> GetAsync(Guid id);

        /// <summary>
        /// Obtiene un mensaje IA por su ID en la plataforma y plataforma.
        /// </summary>
        /// <param name="plataformaMensajeId">Id del mensaje en la plataforma</param>
        /// <param name="plataforma">Plataforma</param>
        /// <returns>MensajeIA con el ID en la plataforma</returns>
        /// <exception cref="NotFoundException"/>
        Task<MensajeIA> GetAsync(string plataformaMensajeId, string plataforma);
    }
}
