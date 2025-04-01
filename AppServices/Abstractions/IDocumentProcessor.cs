using Domain.Entities.DocumentoAgregado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Abstractions
{
    internal interface IDocumentProcessor
    {
        /// <summary>
        /// Procesa el contenido de un archivo de documentación y genera los embeddings
        /// del mismo.
        /// </summary>
        /// <param name="path">Path del documento</param>
        /// <param name="documentData">Bytes del documento</param>
        /// <returns>Contenido del documento con embeddings generados</returns>
        Task<Document> ProcessAsync(string path, byte[] documentData);
    }
}
