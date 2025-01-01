using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Chats
{
    internal class ChatRepository(ChatContext _context) : Repository<Chat>(
        _context), IChatRepository
    {
        public async Task<Chat> GetAsync(
            string usuarioId,
            string chatPlataformaId,
            string plataforma)
        {
            return await _context.Chats
                    .Where(c => c.UsuarioId == usuarioId)
                    .Where(c => c.ChatPlataformaId == chatPlataformaId)
                    .Where(c => c.Plataforma == plataforma)
                    .FirstOrDefaultAsync() ??
                throw new NotFoundException();
        }
    }
}
