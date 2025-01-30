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
        /// Determina si existe un documento de un archivo
        /// </summary>
        /// <param name="filename">Nombre del archivo</param>
        /// <returns>Si existe un documento de un archivo</returns>
        Task<bool> DocumentsWithFilenameAsync(string filename);

        /// <summary>
        /// Obtiene documentos relacionados a un vector de embedding
        /// </summary>
        /// <param name="embedding">Vector de embedding</param>
        /// <returns>Documentos similares al embedding</returns>
        Task<List<Document>> GetDocumentosRelacionadosAsync(
            ReadOnlyMemory<float> embedding);

        /// <summary>
        /// Inserta una lista de documentos
        /// </summary>
        /// <param name="documents">Documentos a insertar</param>
        Task<List<Document>> InsertRangeAsync(List<Document> documents);
    }
}
