using Domain.Entities;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.KernelPlugins
{
    internal class DocumentacionPlugin
    {
        [KernelFunction("Buscar Documentacion")]
        [Description("Busca documentación relevante a la consulta")]
        public async Task<IEnumerable<EmbeddingDoc>> BuscarDocumentacionAsync(
            string consulta)
        {
            throw new NotImplementedException();
        }
    }
}
