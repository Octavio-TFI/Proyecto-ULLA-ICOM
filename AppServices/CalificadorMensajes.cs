using AppServices.Abstractions;
using AppServices.Abstractions.DTOs;
using AppServices.Ports;
using Domain.Entities.ChatAgregado;
using Domain.Exceptions;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    internal class CalificadorMensajes(
        IUnitOfWork unitOfWork,
        IMensajeIARepository mensajeIARepository)
        : ICalificadorMensajes
    {
        readonly IUnitOfWork _unitOfWork = unitOfWork;
        readonly IMensajeIARepository _mensajeIARepository = mensajeIARepository;

        public async Task CalificarMensajeAsync(
            CalificacionMensajeDTO calificacionMensaje)
        {
            MensajeIA mensajeIA = await _mensajeIARepository
                .GetAsync(
                    calificacionMensaje.MensajePlataformaId,
                    calificacionMensaje.Plataforma)
                .ConfigureAwait(false);

            mensajeIA.Calificacion = calificacionMensaje.Calificacion;

            await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
