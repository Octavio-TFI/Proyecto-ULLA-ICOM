﻿using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;
using Domain.Entities;
using Domain.Repositories;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    internal class RecibidorMensajes(IUnitOfWork _unitOfWork) : IRecibidorMensajes
    {
        public async Task RecibirMensajeTextoAsync(MensajeTextoDTO mensaje)
        {
            Chat chat;

            try 
            {
                chat = await _unitOfWork.Chats
                    .GetAsync(
                        mensaje.UsuarioId,
                        mensaje.ChatPlataformaId,
                        mensaje.Plataforma);
            }
            catch (NotFoundException) 
            {
                // Si el chat no existe, se crea uno nuevo
                await _unitOfWork.Chats
                    .InsertAsync(
                        new Chat
                        {
                            UsuarioId = mensaje.UsuarioId,
                            ChatPlataformaId = mensaje.ChatPlataformaId,
                            Plataforma = mensaje.Plataforma
                        });

                // Se guarda el chat para obtener el Id
                await _unitOfWork.SaveChangesAsync();

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

            await _unitOfWork.SaveChangesAsync();
        }
    }
}