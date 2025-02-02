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
        public Task<List<Document>> GetDocumentosRelacionadosAsync(
            ReadOnlyMemory<float> embedding)
        {
            return _context.Documents
                .Include(x => x.Parent)
                .Include(x => x.Childs)
                .OrderBy(
                    x => _context.CosineSimilarity(
                        x.Embedding,
                        embedding.ToArray()))
                .Take(5)
                .ToListAsync();
        }

        public Task<bool> DocumentsWithFilenameAsync(string filename)
        {
            return _context.Documents.AnyAsync(d => d.Filename == filename);
        }
    }
}
