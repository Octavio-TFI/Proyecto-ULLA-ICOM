using Domain.Entities.ChatAgregado;
using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenAI.Responses;
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
                else if (mensaje is MensajeLlamadaHerramienta llamadaHerramienta)
                {
                    var chatMessage = new ChatMessageContent()
                    {
                        Role = AuthorRole.Assistant,
                        Items =
                            [new FunctionCallContent(
                                llamadaHerramienta.FunctionName,
                                llamadaHerramienta.PluginName,
                                llamadaHerramienta.Id.ToString(),
                                new KernelArguments(
                                    llamadaHerramienta.Argumentos!.ToDictionary(
                                        )))]
                    };

                    chatHistory.Add(chatMessage);
                }
                else if (mensaje is MensajeHerramienta mensajeHerramienta)
                {
                    // TODO: ESTA MAL PERO FUNCIONA

                    var functionCall = mensajes.ElementAt(
                        mensajes.IndexOf(mensajeHerramienta) + 1) as MensajeLlamadaHerramienta;

                    var functionResult = new FunctionResultContent(
                        functionCall?.FunctionName,
                        functionCall?.PluginName,
                        functionCall?.Id.ToString(),
                        mensajeHerramienta.Texto);

                    var chatMessage = new ChatMessageContent()
                    {
                        Role = AuthorRole.Tool,
                        Items = [functionResult]
                    };

                    chatHistory.Add(chatMessage);
                }
                else
                {
                    throw new NotSupportedException(
                        $"Tipo de mensaje no soportado: {mensaje.GetType().Name}");
                }
            }

            return chatHistory;
        }
    }
}
