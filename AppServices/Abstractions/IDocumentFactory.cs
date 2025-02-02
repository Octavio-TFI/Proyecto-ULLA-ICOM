﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Abstractions
{
    internal interface IDocumentFactory
    {
        /// <summary>
        /// Crea un documento.
        /// </summary>
        /// <param name="filename">Nombre del archivo</param>
        /// <param name="text">Texto del documento</param>
        /// <param name="parentDocument">Documento padre</param>
        /// <returns>Documento</returns>
        Task<Document> CreateAsync(
            string filename,
            string text,
            Document? parentDocument = null);
    }
}
