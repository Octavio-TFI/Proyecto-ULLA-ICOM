using Domain.Entities.DocumentoAgregado;
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
        public Task<List<Document>> GetDocumentosRelacionadosAsync(
            ReadOnlyMemory<float> embedding)
        {
            return _context.Documents
                .OrderBy(
                    d => d.Chunks
                        .Min(
                            c => _context.CosineDistance(
                                    c.Embedding,
                                    embedding.ToArray())))
                .Take(15)
                .ToListAsync();
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
