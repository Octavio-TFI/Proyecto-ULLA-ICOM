using Domain.Entities;
using Domain.Entities.ConsultaAgregado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Factories
{
    public interface IConsultaFactory
    {
        /// <summary>
        /// Crea una consulta con sus embeddings a partir de los datos de la consulta
        /// </summary>
        /// <param name="consultaData">Datos de la consulta</param>
        /// <returns>Consulta con embeddings generados</returns>
        Task<Consulta> CreateAsync(ConsultaData consultaData);
    }
}
