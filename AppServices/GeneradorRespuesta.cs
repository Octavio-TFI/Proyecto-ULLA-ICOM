using AppServices;
using AppServices.Abstractions;
using AppServices.Ports;
using Domain.Abstractions.Factories;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    internal class GeneradorRespuesta(
        [FromKeyedServices(TipoKernel.GeneradorRepuestas)] Kernel _kernel,
        IMensajeFactory _mensajeFactory,
        IFileManager _fileManager)
        : IGeneradorRespuesta
    {
        public async Task<Mensaje> GenerarRespuestaAsync(List<Mensaje> mensajes)
        {
            ChatHistory chatHistory = [];

            string systemPrompt = await _fileManager.ReadAllTextAsync(
                "Prompts/GeneradorRespuesta/SystemPrompt.txt");

            chatHistory.AddSystemMessage(systemPrompt);

            foreach(Mensaje mensaje in mensajes)
            {
                if(mensaje is MensajeTexto mensajeTexto)
                {
                    if(mensaje.Tipo == TipoMensaje.Usuario)
                    {
                        chatHistory.AddUserMessage(mensajeTexto.Texto);
                    } else if(mensaje.Tipo == TipoMensaje.Asistente)
                    {
                        chatHistory.AddAssistantMessage(mensajeTexto.Texto);
                    }
                }
            }

            var executionSettings = new PromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            };

            var result = await _kernel
                .GetRequiredService<IChatCompletionService>()
                .GetChatMessageContentAsync(
                    chatHistory,
                    executionSettings,
                    _kernel);

            return _mensajeFactory.CreateMensajeTextoGenerado(
                chatId: mensajes.First().ChatId,
                texto: result.ToString());
        }
    }
}
