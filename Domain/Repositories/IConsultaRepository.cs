using Domain.Entities.ConsultaAgregado;
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
        /// Inserta una consulta
        /// </summary>
        /// <param name="consulta">Consulta a insertar</param>
        /// <returns>Consulta insertada</returns>
        Task<Consulta> InsertAsync(Consulta consulta);

        /// <summary>
        /// Obtiene consultas similares a un vector de embedding
        /// </summary>
        /// <param name="embedding">Vector de embedding</param>
        /// <returns>Consultas similares al embedding</returns>
        Task<List<Consulta>> GetConsultasSimilaresAsync(
            ReadOnlyMemory<float> embedding);

        /// <summary>
        /// Obtiene todos los ids de las consultas
        /// </summary>
        /// <returns>Ids de todas las consultas</returns>
        Task<int[]> GetAllIdsAsync();
    }
}
