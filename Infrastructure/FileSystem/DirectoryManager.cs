using AppServices.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FileSystem
{
    internal class DirectoryManager
        : IDirectoryManager
    {
        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }
    }
}
