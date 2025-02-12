using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Embeddings
{
    internal class DocumentRepository(EmbeddingContext _context)
        : Repository<Document>(_context)
        , IDocumentRepository
    {
        public async Task<List<Document>> GetDocumentosRelacionadosAsync(
            ReadOnlyMemory<float> embedding)
        {
            var documents = await _context.Documents
                .OrderBy(
                    d => _context.CosineSimilarity(
                        d.Embedding,
                        embedding.ToArray()))
                .Take(20)
                .ToListAsync();

            return documents;
        }

        public Task<List<string>> GetAllFilenamesAsync()
        {
            return _context.Documents
                .Select(d => d.Filename)
                .Distinct()
                .ToListAsync();
        }
    }
}
