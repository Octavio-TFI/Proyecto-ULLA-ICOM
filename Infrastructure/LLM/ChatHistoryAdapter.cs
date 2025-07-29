using Domain.Entities.ChatAgregado;
using Domain.ValueObjects;
using Infrastructure.LLM.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                    // TODO: ESTA MAL PERO FUNCIONA
                    chatHistory.AddAssistantMessage(
                        "Tool call: " + llamadaHerramienta.ToString());
                }
                else if (mensaje is MensajeHerramienta mensajeHerramienta)
                {
                    // TODO: ESTA MAL PERO FUNCIONA
                    chatHistory.AddMessage(
                        AuthorRole.User,
                        mensajeHerramienta.Texto);
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
