using AppServices.Ports;
using Domain.Events;
using Domain.Repositories;
using MediatR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.EventHandlers
{
    internal class MensajeRecibidoHandler(
        IMensajeRepository _mensajeRepository,
        IGeneradorRespuesta _generadorRespuesta) : INotificationHandler<MensajeRecibidoEvent>
    {
        public async Task Handle(
            MensajeRecibidoEvent request,
            CancellationToken cancellationToken)
        {
            // Obtener ultimos mensajes del chat
            var mensajes = await _mensajeRepository.GetUltimosMensajesChatAsync(
                request.ChatId);

            // Generar respuesta
            var respuesta = await _generadorRespuesta.GenerarRespuestaAsync(
                mensajes);

            // Almacenar respuesta
            await _mensajeRepository.InsertAsync(respuesta);
        }
    }
}
