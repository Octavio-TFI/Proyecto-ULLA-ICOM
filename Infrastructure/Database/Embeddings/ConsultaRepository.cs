using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Embeddings
{
    internal class ConsultaRepository(EmbeddingContext _context)
        : IConsultaRepository
    {
        public Task<List<Consulta>> GetConsultasSimilaresAsync(
            ReadOnlyMemory<float> embedding)
        {
            return _context.Consultas.Take(5).ToListAsync();
        }
    }
}
