using AppServices.Ports;
using Infrastructure.Database.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    internal class UnitOfWork(ChatContext _chatContext) : IUnitOfWork
    {
        public Task SaveChangesAsync()
        {
            return _chatContext.SaveChangesAsync();
        }
    }
}
