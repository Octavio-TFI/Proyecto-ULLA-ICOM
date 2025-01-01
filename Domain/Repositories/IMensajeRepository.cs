using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IMensajeRepository
    {
        /// <summary>
        /// Inserta un mensaje en la DB
        /// </summary>
        /// <param name="mensaje">Mensaje a insertar en la DB</param>
        /// <returns>Mensaje creado</returns>
        Task<Mensaje> InsertAsync(Mensaje mensaje);

        /// <summary>
        /// Obtiene los ultimos 10 mensajes de un chat
        /// </summary>
        /// <param name="chatId">Id del chat</param>
        /// <returns>Ultimos 10 mensajes del chat</returns>
        Task<List<Mensaje>> GetUltimosMensajesChatAsync(int chatId);
    }
}
