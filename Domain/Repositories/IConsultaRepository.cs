using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IConsultaRepository
    {
        /// <summary>
        /// Obtiene consultas similares a un vector de embedding
        /// </summary>
        /// <param name="embedding">Vector de embedding</param>
        /// <returns>Consultas similares al embedding</returns>
        Task<List<Consulta>> GetConsultasSimilaresAsync(
            ReadOnlyMemory<float> embedding);
    }
}
