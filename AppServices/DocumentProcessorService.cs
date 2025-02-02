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

namespace AppServices
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

            var documentPaths = _directoryManager.GetFiles("./Documentacion");

            foreach (var documentPath in documentPaths)
            {
                // Si el documento ya fue procesado, se saltea
                if (await documentRepository.DocumentsWithFilenameAsync(
                    documentPath))
                {
                    _logger.LogInformation(
                        "Documento {documentPath} ya procesado",
                        documentPath);

                    continue;
                }

                _logger.LogInformation(
                    "Procesando documento {documentPath}",
                    documentPath);

                string extension = _pathManager.GetExtension(documentPath);

                var documents = await scopeServices
                    .GetRequiredKeyedService<IDocumentProcessor>(extension)
                    .ProcessAsync(documentPath);

                await documentRepository.InsertRangeAsync(documents);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
