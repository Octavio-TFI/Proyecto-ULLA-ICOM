using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IEmbeddingService
    {
        /// <summary>
        /// Genera vectores de embeddings para el textos dados
        /// </summary>
        /// <param name="textos">Textos</param>
        /// <returns>Embeddings de los textos</returns>
        Task<List<float[]>> GenerateAsync(IList<string> textos);

        /// <summary>
        /// Genera vector de embeddings para el texto dado
        /// </summary>
        /// <param name="texto">Texto</param>
        /// <returns>Embeddings del texto</returns>
        Task<float[]> GenerateAsync(string texto);
    }
}
