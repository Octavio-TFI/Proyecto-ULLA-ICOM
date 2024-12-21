using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;
using AppServices.Ports;
using Domain.Abstractions.Factories;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.ValueObjects;
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
        IMensajeFactory _mensajeFactory,
        IMensajeRepository _mensajeRepository) : IRecibidorMensajes
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
                    mensajeRecibido.Plataforma);
            } catch(NotFoundException)
            {
                // Si el chat no existe, se crea uno nuevo
                chat = await _chatRepository.InsertAsync(
                    new Chat
                    {
                        UsuarioId = mensajeRecibido.UsuarioId,
                        ChatPlataformaId = mensajeRecibido.ChatPlataformaId,
                        Plataforma = mensajeRecibido.Plataforma
                    });

                // Se guarda el chat para obtener el Id
                await _unitOfWork.SaveChangesAsync();
            }

            var mensaje = _mensajeFactory.CreateMensajeTexto(
                chat.Id,
                mensajeRecibido.DateTime,
                TipoMensaje.Usuario,
                mensajeRecibido.Texto);

            await _mensajeRepository.InsertAsync(mensaje);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
