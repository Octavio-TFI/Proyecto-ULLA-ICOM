using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    internal class MensajeRepository(ChatContext _context) 
        : Repository<Mensaje>(_context)
        , IMensajeRepository
    {
    }
}
