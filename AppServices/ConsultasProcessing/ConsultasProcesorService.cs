using AppServices.Abstractions;
using AppServices.Ports;
using Domain;
using Domain.Abstractions.Factories;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppServices.ConsultasProcessing
{
    internal class ConsultasProcesorService(
        IServiceProvider services,
        IConsultaDataRepository consultaDataRepository,
        IConsultaFactory consultaFactory,
        ILogger<ConsultasProcesorService> logger)
        : BackgroundService
    {
        readonly IServiceProvider _services = services;
        readonly IConsultaDataRepository _consultaDataRepository = consultaDataRepository;
        readonly IConsultaFactory _consultaFactory = consultaFactory;
        readonly ILogger<ConsultasProcesorService> _logger = logger;

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessConsultasAsync();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        async Task ProcessConsultasAsync()
        {
            var scopeServices = _services.CreateScope().ServiceProvider;

            var consultasRepository = scopeServices
                .GetRequiredService<IConsultaRepository>();

            var unitOfWork = scopeServices.GetRequiredService<IUnitOfWork>();

            // Se obtienen las consultas de la base de datos de mesa de ayuda
            var existingIds = await consultasRepository.GetAllIdsAsync();
            var consultasDatas = await _consultaDataRepository.GetAllExceptExistingIdsAsync(
                existingIds);

            int i = 1;

            foreach (var consultaData in consultasDatas)
            {
                var stopwatch = Stopwatch.StartNew();

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
                    var consulta = await _consultaFactory.CreateAsync(
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

                // Se espera 4 segundos entre cada consulta
                // Esto es para no sobrecargar la API gratiuta de Gemini
                stopwatch.Stop();
                var elapsed = stopwatch.Elapsed;
                var delay = TimeSpan.FromSeconds(4) - elapsed;

                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay);
                }
            }
        }
    }
}
