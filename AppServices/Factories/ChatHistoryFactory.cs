using AppServices.Abstractions;
using AppServices.Ports;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Factories
{
    internal class ChatHistoryFactory
        : IChatHistoryFactory
    {
        public ChatHistory Create(List<Mensaje> mensajes)
        {
            ChatHistory chatHistory = [];

            foreach (Mensaje mensaje in mensajes)
            {
                if (mensaje is MensajeTexto mensajeTexto)
                {
                    if (mensaje.Tipo == TipoMensaje.Usuario)
                    {
                        chatHistory.AddUserMessage(mensajeTexto.Texto);
                    }
                    else if (mensaje.Tipo == TipoMensaje.Asistente)
                    {
                        chatHistory.AddAssistantMessage(mensajeTexto.Texto);
                    }
                }
            }

            return chatHistory;
        }
    }
}
