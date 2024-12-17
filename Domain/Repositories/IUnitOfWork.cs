using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Repositorio de chats
        /// </summary>
        IChatRepository Chats { get; }

        /// <summary>
        /// Repositorio de mensajes
        /// </summary>
        IMensajeRepository Mensajes { get; }

        /// <summary>
        /// Guarda los cambios en la DB
        /// </summary>
        Task SaveChangesAsync();
    }
}
