using AppServices.Abstractions;
using AppServices.Ports;
using Domain;
using Domain.Repositories;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.DocumentProcessing
{
    internal class DocumentProcessorService(
        IServiceProvider _services,
        IDirectoryManager _directoryManager,
        IPathManager _pathManager,
        ILogger<DocumentProcessorService> _logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var scopeServices = _services.CreateScope().ServiceProvider;

            var documentRepository = scopeServices
                .GetRequiredService<IDocumentRepository>();

            var unitOfWork = scopeServices
                .GetRequiredKeyedService<IUnitOfWork>(Contexts.Embedding);

            var documentPaths = _directoryManager.GetFiles("Documentacion");

            var dbDocumentsPaths = await documentRepository
                .GetAllFilenamesAsync();

            documentPaths = documentPaths.Except(dbDocumentsPaths).ToArray();
            int i = 1;

            foreach (var documentPath in documentPaths)
            {
                _logger.LogInformation(
                    "Procesando {documentPath} {i}/{count}",
                    documentPath,
                    i++,
                    documentPaths.Length);

                try
                {
                    string extension = _pathManager.GetExtension(documentPath)
                        .ToLower();

                    var documents = await scopeServices
                        .GetRequiredKeyedService<IDocumentProcessor>(extension)
                        .ProcessAsync(documentPath);

                    await documentRepository.InsertRangeAsync(documents);
                    await unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error al procesar {documentPath}",
                        documentPath);
                }
            }

            _logger.LogInformation("Procesamiento de documentos finalizado");
        }
    }
}
