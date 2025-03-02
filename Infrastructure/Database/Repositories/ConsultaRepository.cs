using Domain.Entities.ConsultaAgregado;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Repositories
{
    internal class ConsultaRepository(ChatContext context)
        : Repository<Consulta>(context)
        , IConsultaRepository
    {
        readonly ChatContext _context = context;

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
