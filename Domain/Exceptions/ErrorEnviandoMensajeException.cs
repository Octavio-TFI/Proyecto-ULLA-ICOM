using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ErrorEnviandoMensajeException(
        string? message = null,
        Exception? innerException = null)
        : Exception(message, innerException)
    {
    }
}
