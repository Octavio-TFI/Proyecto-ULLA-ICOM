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
        public Task<List<int>> GetAllIdsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Consulta>> GetConsultasSimilaresAsync(
            ReadOnlyMemory<float> embedding)
        {
            var embeddingArray = embedding.ToArray();

            // Obtener por similitud de embedding con titulo
            var consultasTituloTask = _context.Consultas
                .Select(
                    x => new VectorSearchResult<Consulta>
                    {
                        Distance =
                            _context.CosineDistance(
                                    x.EmbeddingTitulo,
                                    embeddingArray),
                        Entity = x
                    })
                .OrderBy(x => x.Distance)
                .Take(5)
                .ToListAsync();

            // Obtener por similitud de embedding con descripcion
            var consultasDescripcionTask = _context.Consultas
                .Select(
                    x => new VectorSearchResult<Consulta>
                    {
                        Distance =
                            _context.CosineDistance(
                                    x.EmbeddingDescripcion,
                                    embeddingArray),
                        Entity = x
                    })
                .OrderBy(x => x.Distance)
                .Take(5)
                .ToListAsync();

            await Task.WhenAll(consultasTituloTask, consultasDescripcionTask);

            // Unir consultas
            return consultasTituloTask.Result
                .Union(consultasDescripcionTask.Result)
                .OrderBy(c => c.Distance)
                .Select(c => c.Entity)
                .Take(5)
                .ToList();
        }

        public Task<Consulta> InsertAsync(Consulta consulta)
        {
            throw new NotImplementedException();
        }
    }
}
