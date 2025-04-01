using Domain.Entities.DocumentoAgregado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Factories
{
    public interface IDocumentFactory
    {
        /// <summary>
        /// Crea un documento.
        /// </summary>
        /// <param name="filename">Nombre del archivo</param>
        /// <param name="text">Texto del documento</param>
        /// <param name="textChunks">Textos de los pedazos del documento</param>
        /// <returns>Documento</returns>
        Task<Document> CreateAsync(
            string filename,
            string text,
            IList<string> textChunks);
    }
}
