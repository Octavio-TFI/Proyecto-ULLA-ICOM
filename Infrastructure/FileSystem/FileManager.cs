using AppServices.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FileSystem
{
    internal class FileManager
        : IFileManager
    {
        public Task<byte[]> ReadAllBytesAsync(string path)
        {
            return File.ReadAllBytesAsync(path);
        }

        public Task<string> ReadAllTextAsync(string path)
        {
            return File.ReadAllTextAsync(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }
    }
}
