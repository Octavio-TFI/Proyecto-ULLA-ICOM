using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Ports
{
    public interface IPathManager
    {
        /// <summary>
        /// Obtiene la extension de un archivo
        /// </summary>
        /// <param name="path">Path del archivo</param>
        /// <returns>Extension del archivo</returns>
        string GetExtension(string path);
    }
}
