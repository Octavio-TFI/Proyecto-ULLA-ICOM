using Domain.Entities.ChatAgregado;
using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LLM
{
    internal class ChatHistoryAdapter
        : IChatHistoryAdapter
    {
        public ChatHistory Adapt(List<Mensaje> mensajes)
        {
            ChatHistory chatHistory = [];

            foreach (Mensaje mensaje in mensajes.OrderBy(m => m.DateTime))
            {
                if (mensaje is MensajeTextoUsuario mensajeTexto)
                {
                    chatHistory.AddUserMessage(mensajeTexto.Texto);
                }
                else if (mensaje is MensajeIA mensajeIA)
                {
                    chatHistory.AddAssistantMessage(mensajeIA.Texto);
                }
            }

            return chatHistory;
        }
    }
}
