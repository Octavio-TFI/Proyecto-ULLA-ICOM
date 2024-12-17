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
        /// Obtiene un chat
        /// </summary>
        /// <param name="usuarioId">Id del usuario que creo el chat</param>
        /// <param name="chatId">Id del chat en la plataforma</param>
        /// <param name="plataforma">Plataforma de mensajeria</param>
        /// <returns>Chat o null si no se encontro</returns>
        Task<Chat?> GetAsync(string usuarioId, string chatId, string plataforma);
    }
}
