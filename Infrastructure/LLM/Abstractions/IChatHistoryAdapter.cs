using Domain.Entities;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.Abstractions
{
    internal interface IChatHistoryAdapter
    {
        /// <summary>
        /// Adapta una lista de mensajes a un ChatHistory
        /// </summary>
        /// <param name="mensajes">Lista de mensajes</param>
        /// <returns>ChatHistory con mensajes</returns>
        ChatHistory Adapt(List<Mensaje> mensajes);
    }
}
