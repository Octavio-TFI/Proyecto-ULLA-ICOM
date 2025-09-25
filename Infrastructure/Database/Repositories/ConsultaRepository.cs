using Domain.Entities.ConsultaAgregado;
using Domain.Repositories;
using iText.Pdfua.Checkers.Utils;
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

        private class ConsultaDistancia
        {
            public required Consulta Consulta { get; set; }

            public required double Distance { get; set; }
        }

        public async Task<List<Consulta>> GetConsultasSimilaresAsync(
            ReadOnlyMemory<float> embedding)
        {
            var embeddingArray = embedding.ToArray();

            var consultasPorTitulo = await _context.Consultas
                .Select(
                    consulta => new ConsultaDistancia
                    {
                        Consulta = consulta,
                        Distance =
                            EF.Functions
                                    .VectorDistance(
                                        "cosine",
                                        consulta.EmbeddingTitulo,
                                        embeddingArray)
                    })
                .OrderBy(x => x.Distance)
                .Take(10)
                .ToListAsync();

            var consultasPorDescripcion = await _context.Consultas
                .Select(
                    consulta => new ConsultaDistancia
                    {
                        Consulta = consulta,
                        Distance =
                            EF.Functions
                                    .VectorDistance(
                                        "cosine",
                                        consulta.EmbeddingDescripcion,
                                        embeddingArray)
                    })
                .OrderBy(x => x.Distance)
                .Take(10)
                .ToListAsync();

            return[ .. consultasPorDescripcion.Concat(consultasPorTitulo)
                .OrderBy(c => c.Distance)
                .Select(c => c.Consulta)
                .DistinctBy(c => c.Id)
                .Take(10) ];
        }

        public Task<string> GetTextoAsync(Guid guid)
        {
            return _context.Consultas
                .Where(c => c.Id == guid)
                .Select(c => c.ToString())
                .FirstAsync();
        }
    }
}
