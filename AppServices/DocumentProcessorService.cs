using AppServices.Abstractions;
using AppServices.Ports;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        IDocumentRepository _documentRepository,
        IUnitOfWork _unitOfWork)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var documentPaths = _directoryManager.GetFiles("./Documentacion");

            foreach (var documentPath in documentPaths)
            {
                var documents = await _services.GetRequiredKeyedService<IDocumentProcessor>(
                    "pdf")
                    .ProcessAsync(documentPath);

                await _documentRepository.InsertRangeAsync(documents);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
