using Domain.ValueObjects;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM.Abstractions
{
    internal interface IToolCallExtractor
    {
        /// <summary>
        /// Extrae las llamadas a funciones de un mensaje de chat.
        /// </summary>
        /// <param name="chatMessage">Mensaje de chat</param>
        /// <returns>Llamadas a funciones en este mensaje</returns>
        IEnumerable<AgentFunctionCall> Extract(ChatMessageContent chatMessage);
    }
}
