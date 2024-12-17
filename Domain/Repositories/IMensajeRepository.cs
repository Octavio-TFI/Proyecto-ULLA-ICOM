using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    internal interface IMensajeRepository
    {
        /// <summary>
        /// Guarda un mensaje en la DB
        /// </summary>
        /// <param name="mensaje">Mensaje a guardar en la DB</param>
        Task SaveAsync(Mensaje mensaje);
    }
}
