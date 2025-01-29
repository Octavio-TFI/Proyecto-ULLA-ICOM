using AppServices.Ports;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    internal class DocumentProcessorService(IDirectoryManager _directoryManager)
        : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var documentPaths = _directoryManager.GetFiles("./Documentacion");

            return Task.CompletedTask;
        }
    }
}
