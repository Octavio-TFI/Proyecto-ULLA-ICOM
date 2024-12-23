using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public enum TipoMensaje
    {
        Indefinido,
        // Mensaje enviado por el usuario
        Usuario,
        // Mensaje generado por el asistente
        Asistente
    }
}
