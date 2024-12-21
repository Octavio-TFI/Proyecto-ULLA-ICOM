using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    internal class ChatRepository : IChatRepository
    {
        public Task<Chat> GetAsync(
            string usuarioId,
            string chatId,
            string plataforma)
        {
            throw new NotImplementedException();
        }

        public Task<Chat> InsertAsync(Chat chat)
        {
            throw new NotImplementedException();
        }
    }
}
