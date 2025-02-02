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
                .Include(d => d.Parent)
                .Include(d => d.Childs)
                .OrderBy(
                    d => _context.CosineSimilarity(
                        d.Embedding,
                        embedding.ToArray()))
                .Select(d => GetTopParent(d))
                .Take(5)
                .ToListAsync();

            return documents.Distinct().ToList();
        }

        static Document GetTopParent(Document document)
        {
            if (document.Parent is null)
            {
                return document;
            }

            return GetTopParent(document.Parent);
        }

        public Task<bool> DocumentsWithFilenameAsync(string filename)
        {
            return _context.Documents.AnyAsync(d => d.Filename == filename);
        }
    }
}
