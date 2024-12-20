using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Ports
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Guarda los cambios en la DB
        /// </summary>
        Task SaveChangesAsync();
    }
}
