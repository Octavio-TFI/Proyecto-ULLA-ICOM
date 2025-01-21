using AppServices.Ports;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Clients
{
    internal class TestClient
        : IClient
    {
        public Task EnviarMensaje(
            string chatPlataformaId,
            string usuarioId,
            Mensaje mensaje)
        {
            throw new NotImplementedException();
        }
    }
}
