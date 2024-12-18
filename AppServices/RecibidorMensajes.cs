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
                    mensaje.ChatPlataformaId,
                    mensaje.Plataforma);

            // Si el chat no existe, se crea uno nuevo
            if (chat is null)
            {
                await _unitOfWork.Chats
                    .InsertAsync(
                        new Chat
                        {
                            UsuarioId = mensaje.UsuarioId,
                            ChatPlataformaId = mensaje.ChatPlataformaId,
                            Plataforma = mensaje.Plataforma
                        });

                chat = await _unitOfWork.Chats
                    .GetAsync(
                        mensaje.UsuarioId,
                        mensaje.ChatPlataformaId,
                        mensaje.Plataforma);
            }

            await _unitOfWork.Mensajes
                .InsertAsync(
                    new MensajeTexto
                    {
                        ChatId = chat.Id,
                        DateTime = mensaje.DateTime,
                        Texto = mensaje.Texto
                    });
        }
    }
}
