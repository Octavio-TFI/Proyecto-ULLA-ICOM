using AppServices.Abstractions;
using AppServices.Ports;
using Domain;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.ConsultasProcessing
{
    internal class ConsultasProcesorService(
        IServiceProvider services,
        IConsultaDataRepository consultaDataRepository,
        IConsultaProcessor _consultaProcessor,
        ILogger<ConsultasProcesorService> logger)
        : BackgroundService
    {
        readonly IServiceProvider _services = services;
        readonly IConsultaDataRepository _consultaDataRepository = consultaDataRepository;
        readonly ILogger<ConsultasProcesorService> _logger = logger;

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var scopeServices = _services.CreateScope().ServiceProvider;

            var consultasRepository = scopeServices
                .GetRequiredService<IConsultaRepository>();

            var unitOfWork = scopeServices
                .GetRequiredKeyedService<IUnitOfWork>(Contexts.Embedding);

            // Se obtienen las consultas de la base de datos de mesa de ayuda
            var consultasDatas = await _consultaDataRepository.GetAllAsync();

            // Consultas ya procesadas se saltean
            var ids = await consultasRepository.GetAllIdsAsync();
            consultasDatas = consultasDatas.ExceptBy(ids, x => x.Id).ToList();

            int i = 1;

            foreach (var consultaData in consultasDatas)
            {
                _logger.LogInformation(
                    @"Procesando consulta
Id: {id}
Titulo: {titulo}
{i}/{count}",
                    consultaData.Id,
                    consultaData.Titulo,
                    i++,
                    consultasDatas.Count);

                try
                {
                    var consulta = await _consultaProcessor.ProcessAsync(
                        consultaData);

                    await consultasRepository.InsertAsync(consulta);
                    await unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        @"Error al procesar la consulta:
Id: {id}
Titulo: {titulo}",
                        consultaData.Id,
                        consultaData.Titulo);
                }
            }
        }
    }
}
