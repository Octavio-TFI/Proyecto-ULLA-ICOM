﻿using System;
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

        /// <summary>
        /// Lee el contenido de un archivo
        /// </summary>
        /// <param name="path">Path del archivo</param>
        /// <returns>Texto del archivo</returns>
        string ReadAllText(string path);

        /// <summary>
        /// Lee el contenido de un archivo en bytes
        /// </summary>
        /// <param name="path">Path del archivo</param>
        /// <returns>Bytes del archivo</returns>
        Task<byte[]> ReadAllBytesAsync(string path);

        /// <summary>
        /// Elimina un archivo
        /// </summary>
        /// <param name="path">Path del archivo</param>
        void Delete(string path);
    }
}
