using Domain.Entities;
using Domain.Exceptions;
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
        /// <returns>Chat de la flataforma indicada con estos Ids</returns>
        /// <exception cref="NotFoundException"/>
        Task<Chat> GetAsync(string usuarioId, string chatId, string plataforma);

        /// <summary>
        /// Inserta un chat en la DB
        /// </summary>
        /// <param name="chat">Chat a insertar en la DB</param>
        Task InsertAsync(Chat chat);
    }
}
