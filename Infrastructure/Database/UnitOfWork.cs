using AppServices.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    internal class UnitOfWork(ChatContext _context) : IUnitOfWork
    {
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
