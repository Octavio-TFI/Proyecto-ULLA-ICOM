using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;
using AppServices.Ports;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    internal class RecibidorMensajes(
        IUnitOfWork _unitOfWork,
        IChatRepository _chatRepository,
        IMensajeRepository _mensajeRepository)
        : IRecibidorMensajes
    {
        public async Task RecibirMensajeTextoAsync(MensajeTextoDTO mensaje)
        {
            Chat chat;

            try
            {
                chat = await _chatRepository.GetAsync(
                    mensaje.UsuarioId,
                    mensaje.ChatPlataformaId,
                    mensaje.Plataforma);
            }
            catch (NotFoundException)
            {
                // Si el chat no existe, se crea uno nuevo
                chat = await _chatRepository.InsertAsync(
                    new Chat
                    {
                        UsuarioId = mensaje.UsuarioId,
                        ChatPlataformaId = mensaje.ChatPlataformaId,
                        Plataforma = mensaje.Plataforma
                    });

                // Se guarda el chat para obtener el Id
                await _unitOfWork.SaveChangesAsync();
            }

            await _mensajeRepository.InsertAsync(
                new MensajeTexto
                {
                    ChatId = chat.Id,
                    DateTime = mensaje.DateTime,
                    Texto = mensaje.Texto
                });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
