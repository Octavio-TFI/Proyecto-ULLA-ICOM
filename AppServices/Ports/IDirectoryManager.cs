using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Ports
{
    public interface IDirectoryManager
    {
        /// <summary>
        /// Obtiene los archivos de un directorio
        /// </summary>
        /// <param name="path">Path del directorio</param>
        /// <returns>Archivos en el directorio</returns>
        string[] GetFiles(string path);

        /// <summary>
        /// Verifica si un directorio existe
        /// </summary>
        /// <param name="path">Path del directorio</param>
        /// <returns>True si el directorio existe, false en caso contrario</returns>
        bool Exists(string path);
    }
}
