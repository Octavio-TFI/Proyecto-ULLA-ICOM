using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IConsultaDataRepository
    {
        /// <summary>
        /// Obtiene todas las consultas de la base de datos de mesa de ayuda excepto de
        /// las consultas con los ids especificados
        /// </summary>
        /// <param name="existingIds">Id de consultas ya existentes</param>
        /// <returns>
        /// Todas las consultas de la base de datos de mesa de ayuda excepto de las
        /// consultas con los ids especificados
        /// </returns>
        Task<List<ConsultaData>> GetAllExceptExistingIdsAsync(int[] existingIds);
    }
}
