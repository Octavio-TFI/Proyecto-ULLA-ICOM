using AppServices.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FileSystem
{
    internal class PathManager
        : IPathManager
    {
        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }
    }
}
