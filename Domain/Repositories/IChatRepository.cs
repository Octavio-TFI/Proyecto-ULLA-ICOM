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
        /// <param name="id">Id del chat</param>
        /// <returns>Chat con el id indicado</returns>
        /// <exception cref="NotFoundException"/>
        Task<Chat> GetAsync(int id);

        /// <summary>
        /// Obtiene un chat de la DB con los ultimos 10 mensajes
        /// </summary>
        /// <param name="id">Id del chat</param>
        /// <returns>Chat con el id indicado y los ultimos 10 mensajes</returns>
        /// <exception cref="NotFoundException"/>
        Task<Chat> GetWithUltimosMensajesAsync(int id);

        /// <summary>
        /// Obtiene un chat de la DB
        /// </summary>
        /// <param name="usuarioId">Id del usuario que creo el chat</param>
        /// <param name="chatPlataformaId">Id del chat en la plataforma</param>
        /// <param name="plataforma">Plataforma de mensajeria</param>
        /// <returns>Chat de la flataforma indicada con estos Ids</returns>
        /// <exception cref="NotFoundException"/>
        Task<Chat> GetAsync(
            string usuarioId,
            string chatPlataformaId,
            string plataforma);

        /// <summary>
        /// Inserta un chat en la DB
        /// </summary>
        /// <param name="chat">Chat a insertar en la DB</param>
        /// <returns>Chat creado</returns>
        Task<Chat> InsertAsync(Chat chat);
    }
}
