using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;
using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    public class RecibidorMensajes(IUnitOfWork _unitOfWork) : IRecibidorMensajes
    {
        public async Task RecibirMensajeTextoAsync(MensajeTextoDTO mensaje)
        {
            var chat = await _unitOfWork.Chats
                .GetAsync(
                    mensaje.UsuarioId,
                    mensaje.ChatId,
                    mensaje.Plataforma);

            if (chat is null)
            {
                throw new ArgumentException("Chat no encontrado");
            }

            await _unitOfWork.Mensajes
                .SaveAsync(
                    new MensajeTexto
                    {
                        ChatId = chat.Id,
                        DateTime = mensaje.DateTime,
                        Texto = mensaje.Texto
                    });
        }
    }
}
