using Domain.Entities.ChatAgregado;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Repositories
{
    internal class MensajeIARepository
        : IMensajeIARepository
    {
        public Task<MensajeIA> GetAsync(
            string mensajePlataformaId,
            string plataforma)
        {
            throw new NotImplementedException();
        }
    }
}
