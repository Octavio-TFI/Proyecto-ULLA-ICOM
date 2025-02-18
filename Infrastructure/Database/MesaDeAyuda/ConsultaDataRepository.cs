using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.MesaDeAyuda
{
    internal class ConsultaDataRepository
        : IConsultaDataRepository
    {
        public Task<List<ConsultaData>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
