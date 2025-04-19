using Domain.Entities.ChatAgregado;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Repositories
{
    internal class MensajeIARepository(ChatContext context)
        : IMensajeIARepository
    {
        readonly ChatContext _context = context;

        public async Task<MensajeIA> GetAsync(
            string plataformaMensajeId,
            string plataforma)
        {
            return await _context.Set<MensajeIA>()
                    .Where(
                        m => m.PlataformaMensajeId == plataformaMensajeId &&
                                _context.Chats
                                    .First(c => c.Mensajes.Contains(m))
                                    .Plataforma ==
                                plataforma)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false) ??
                throw new NotFoundException();
        }
    }
}
