using AppServices.Abstractions;
using AppServices.Ports;
using Domain;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.EventHandlers
{
    internal class MensajeRecibidoHandler(
        IMensajeRepository _mensajeRepository,
        IGeneradorRespuesta _generadorRespuesta,
        [FromKeyedServices(Contexts.Chat)] IUnitOfWork _unitOfWork,
        ILogger<MensajeGeneradoHandler> _logger)
        : INotificationHandler<MensajeRecibidoEvent>
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
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                @"
MENSAJE GENERADO
Texto: {Texto}
ChatId: {ChatId}",
                respuesta.ToString(),
                respuesta.ChatId);
        }
    }
}
