using AppServices.Ports;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FileSystem
{
    public static class FileSystemServiceRegistration
    {
        public static IServiceCollection AddFileManagerServices(
            this IServiceCollection services)
        {
            return services
                .AddSingleton<IDirectoryManager, DirectoryManager>()
                .AddSingleton<IPathManager, PathManager>()
                .AddSingleton<IFileManager, FileManager>();
        }
    }
}
