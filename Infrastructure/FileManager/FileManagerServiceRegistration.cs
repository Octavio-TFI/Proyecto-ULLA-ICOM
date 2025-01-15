using AppServices.Ports;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FileManager
{
    public static class FileManagerServiceRegistration
    {
        public static IServiceCollection AddFileManagerServices(
            this IServiceCollection services)
        {
            return services.AddSingleton<IFileManager, FileManager>();
        }
    }
}
