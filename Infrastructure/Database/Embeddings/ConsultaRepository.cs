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
    internal class ConsultaRepository(EmbeddingContext context)
        : Repository<Consulta>(context)
        , IConsultaRepository
    {
        readonly EmbeddingContext _context = context;

        public Task<int[]> GetAllIdsAsync()
        {
            return _context.Consultas.Select(c => c.RemoteId).ToArrayAsync();
        }

        public Task<List<Consulta>> GetConsultasSimilaresAsync(
            ReadOnlyMemory<float> embedding)
        {
            var embeddingArray = embedding.ToArray();

            return _context.Consultas
                .OrderBy(
                    x => Math.Min(
                        _context.CosineDistance(
                            x.EmbeddingTitulo,
                            embeddingArray),
                        _context.CosineDistance(
                            x.EmbeddingDescripcion,
                            embeddingArray)))
                .Take(5)
                .ToListAsync();
        }
    }
}
