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
        /// Obtiene todas las consultas de la base de datos de mesa de ayuda
        /// </summary>
        /// <returns>Todas las consultas de la base de datos de mesa de ayuda</returns>
        Task<List<ConsultaData>> GetAllAsync();
    }
}
