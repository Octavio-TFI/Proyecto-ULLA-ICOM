using AppServices.Abstractions;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.DocumentProcessors
{
    internal class ChmDocumentProcessor
        : IDocumentProcessor
    {
        public Task<List<Document>> ProcessAsync(string path)
        {
            throw new NotImplementedException();
        }
    }
}
