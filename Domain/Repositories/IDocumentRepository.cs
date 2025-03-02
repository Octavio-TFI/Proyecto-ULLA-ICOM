using Domain.Entities.DocumentoAgregado;
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
        /// Obtiene todos los nombres de los documentos en la base de datos
        /// </summary>
        /// <returns>Nombres de los archivos todos los documentos</returns>
        Task<List<string>> GetAllFilenamesAsync();

        /// <summary>
        /// Obtiene documentos relacionados a un vector de embedding
        /// </summary>
        /// <param name="embedding">Vector de embedding</param>
        /// <returns>Documentos similares al embedding</returns>
        Task<List<Document>> GetDocumentosRelacionadosAsync(
            ReadOnlyMemory<float> embedding);

        /// <summary>
        /// Inserta un documento
        /// </summary>
        /// <param name="document">Documento a insertar</param>
        /// <returns>Documento insertado</returns>
        Task<Document> InsertAsync(Document document);
    }
}
