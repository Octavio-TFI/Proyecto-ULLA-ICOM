﻿using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Embeddings
{
    internal class DocumentRepository(EmbeddingContext _context)
        : IDocumentRepository
    {
        public Task<List<Document>> GetDocumentosRelacionadosAsync(
            ReadOnlyMemory<float> embedding)
        {
            return _context.Documents
                .OrderBy(
                    x => _context.CosineSimilarity(
                        x.Embedding,
                        embedding.ToArray()))
                .Take(5)
                .ToListAsync();
        }
    }
}
