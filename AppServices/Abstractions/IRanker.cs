using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Abstractions
{
    /// <summary>
    /// Interfaz para rankear los datos recuperados de una busqueda vectorial.
    /// </summary>
    internal interface IRanker
    {
        /// <summary>
        /// Elimina los datos recuperados que no sean relevantes a la consulta.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datosRecuperados">Datos recuperados con busqueda vectorial</param>
        /// <param name="consulta">Consulta realizada por el usuario</param>
        /// <returns>Datos recuperados filtrados</returns>
        public Task<List<T>> Rank<T>(List<T> datosRecuperados, string consulta)
            where T : Entity;
    }
}
