using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;
using AppServices.Ports;
using Domain;
using Domain.Abstractions.Factories;
using Domain.Entities.ChatAgregado;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    internal class RecibidorMensajes(
        [FromKeyedServices(Contexts.Chat)] IUnitOfWork _unitOfWork,
        IChatRepository _chatRepository)
        : IRecibidorMensajes
    {
        public async Task RecibirMensajeTextoAsync(
            MensajeTextoRecibidoDTO mensajeRecibido)
        {
            Chat chat;

            try
            {
                chat = await _chatRepository.GetAsync(
                    mensajeRecibido.UsuarioId,
                    mensajeRecibido.ChatPlataformaId,
                    mensajeRecibido.Plataforma)
                    .ConfigureAwait(false);
            } catch (NotFoundException)
            {
                // Si el chat no existe, se crea uno nuevo
                chat = await _chatRepository.InsertAsync(
                    new Chat
                    {
                        UsuarioId = mensajeRecibido.UsuarioId,
                        ChatPlataformaId = mensajeRecibido.ChatPlataformaId,
                        Plataforma = mensajeRecibido.Plataforma
                    })
                    .ConfigureAwait(false);
            }

            chat.AñadirMensajeTextoRecibido(
                mensajeRecibido.DateTime,
                mensajeRecibido.Texto);

            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
