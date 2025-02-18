using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Abstractions
{
    internal interface IConsultaProcessor
    {
        /// <summary>
        /// Procesa los datos de una consulta y genera los embeddings
        /// </summary>
        /// <param name="consultaData">Datos de la consulta</param>
        /// <returns>Consulta con embeddings generados</returns>
        Task<Consulta> ProcessAsync(ConsultaData consultaData);
    }
}
