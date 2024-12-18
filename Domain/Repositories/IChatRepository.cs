using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IChatRepository
    {
        /// <summary>
        /// Obtiene un chat de la DB
        /// </summary>
        /// <param name="usuarioId">Id del usuario que creo el chat</param>
        /// <param name="chatId">Id del chat en la plataforma</param>
        /// <param name="plataforma">Plataforma de mensajeria</param>
        /// <returns>Chat o null si no se encontro</returns>
        Task<Chat?> GetAsync(string usuarioId, string chatId, string plataforma);

        /// <summary>
        /// Inserta un chat en la DB
        /// </summary>
        /// <param name="chat">Chat a insertar en la DB</param>
        /// <returns>Chat insertado en la DB</returns>
        Task<Chat> InsertAsync(Chat chat);
    }
}
