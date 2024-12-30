using AppServices.Ports;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM
{
    internal class GeneradorRespuesta : IGeneradorRespuesta
    {
        public Task<Mensaje> GenerarRespuestaAsync(List<Mensaje> mensajes)
        {
            throw new NotImplementedException();
        }
    }
}
