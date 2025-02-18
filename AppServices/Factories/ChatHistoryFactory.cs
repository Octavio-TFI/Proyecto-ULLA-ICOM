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
    internal class ChatHistoryFactory(IFileManager _fileManager)
        : IChatHistoryFactory
    {
        public async Task<ChatHistory> CreateAsync(List<Mensaje> mensajes)
        {
            ChatHistory chatHistory = [];

            string systemPrompt = await _fileManager.ReadAllTextAsync(
                "Prompts/GeneradorRespuesta/SystemPrompt.txt");

            chatHistory.AddSystemMessage(systemPrompt);

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
