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
        Task InsertAsync(Mensaje mensaje);
    }
}
