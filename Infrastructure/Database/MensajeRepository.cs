using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    internal class MensajeRepository : IMensajeRepository
    {
        public Task<Mensaje> InsertAsync(Mensaje mensaje)
        {
            throw new NotImplementedException();
        }
    }
}
