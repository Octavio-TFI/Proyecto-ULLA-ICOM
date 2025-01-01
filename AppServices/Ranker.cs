using AppServices.Abstractions;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    internal class Ranker
        : IRanker
    {
        public Task<List<T>> RankAsync<T>(
            List<T> datosRecuperados,
            string consulta)
            where T : Entity
        {
            throw new NotImplementedException();
        }
    }
}
