using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Ports
{
    /// <summary>
    /// Interfaz para la gestión de archivos
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Lee el contenido de un archivo
        /// </summary>
        /// <param name="path">Path del archivo</param>
        /// <returns>Texto del archivo</returns>
        Task<string> ReadAllTextAsync(string path);
    }
}
