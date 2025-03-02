using Domain.Entities.ChatAgregado;
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
    internal class ChatRepository(ChatContext _context)
        : Repository<Chat>(_context)
        , IChatRepository
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
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false) ??
                throw new NotFoundException();
        }

        public async Task<Chat> GetWithUltimosMensajesAsync(Guid id)
        {
            return await _context.Chats
                    .Include(
                        c => c.Mensajes.OrderByDescending(m => m.DateTime).Take(10))
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false) ??
                throw new NotFoundException();
        }
    }
}
