using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    internal class CalificadorMensajes
        : ICalificadorMensajes
    {
        public Task CalificarMensajeAsync(
            CalificacionMensajeDTO calificacionMensaje)
        {
            throw new NotImplementedException();
        }
    }
}
