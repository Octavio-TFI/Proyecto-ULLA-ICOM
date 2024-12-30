using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IDocumentRepository
    {
        /// <summary>
        /// Obtiene documentos relacionados a un vector de embedding
        /// </summary>
        /// <param name="embedding">Vector de embedding</param>
        /// <returns>Documentos similares al embedding</returns>
        Task<List<Document>> GetDocumentosRelacionadosAsync(
            ReadOnlyMemory<float> embedding);
    }
}
