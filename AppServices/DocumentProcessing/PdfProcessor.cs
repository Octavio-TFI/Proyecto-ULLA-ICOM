using AppServices.Abstractions;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.DocumentProcessing
{
    internal class PdfProcessor
        : IDocumentProcessor
    {
        public Task<List<Document>> ProcessAsync(
            string path,
            byte[] documentData)
        {
            throw new NotImplementedException();
        }
    }
}
