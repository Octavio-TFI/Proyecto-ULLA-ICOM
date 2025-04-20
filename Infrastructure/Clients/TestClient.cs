using AppServices.Abstractions;
using AppServices.Ports;
using Domain.Abstractions.Entities;
using Domain.Entities.ChatAgregado;
using Domain.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Clients
{
    internal class TestClient(IHttpClientFactory _httpClientFactory)
        : IClient
    {
        public async Task<string> EnviarMensajeAsync(
            string chatPlataformaId,
            string usuarioId,
            MensajeIA mensaje)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(Platforms.Test);

                HttpResponseMessage response = await EnviarMensajeAsync(
                    client,
                    chatPlataformaId,
                    mensaje);

                if (!response.IsSuccessStatusCode)
                {
                    throw new ErrorEnviandoMensajeException(
                        $"Status code no exitoso: {response.StatusCode}");
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new ErrorEnviandoMensajeException(innerException: ex);
            }
        }

        static async Task<HttpResponseMessage> EnviarMensajeAsync(
            HttpClient client,
            string chatPlataformaId,
            Mensaje mensaje)
        {
            if (mensaje is IMensajeTexto mensajeTexto)
            {
                return await client.PostAsync(
                    "/Chat",
                    JsonContent.Create(
                        new TestDTO
                    {
                        ChatId = chatPlataformaId,
                        Texto = mensajeTexto.Texto
                    }));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

