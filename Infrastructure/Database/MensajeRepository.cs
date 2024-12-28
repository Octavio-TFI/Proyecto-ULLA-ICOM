using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    internal class MensajeRepository(ChatContext _context) : Repository<Mensaje>(
        _context), IMensajeRepository
    {
        public Task<List<Mensaje>> GetUltimosMensajesChatAsync(int chatId)
        {
            return _context.Mensajes
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.DateTime)
                .Take(10)
                .ToListAsync();
        }
    }
}
