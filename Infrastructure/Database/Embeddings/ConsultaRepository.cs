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
            var embeddingArray = embedding.ToArray();

            // Ordenar por similitud de embedding con titulo y descripcion
            return _context.Consultas
                .OrderBy(
                    x => _context.CosineSimilarity(
                            x.EmbeddingTitulo,
                            embeddingArray) +
                        _context.CosineSimilarity(
                            x.EmbeddingDescripcion,
                            embeddingArray))
                .Take(5)
                .ToListAsync();
        }
    }
}
