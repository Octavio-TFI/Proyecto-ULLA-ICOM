using AppServices.Abstractions;
using AppServices.Ports;
using Domain.Entities;
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
        public async Task EnviarMensajeAsync(
            string chatPlataformaId,
            string usuarioId,
            Mensaje mensaje)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(Platforms.Test);

                HttpResponseMessage response = await EnviarMensajeAsync(
                    client,
                    chatPlataformaId,
                    mensaje);

                if(!response.IsSuccessStatusCode)
                {
                    throw new ErrorEnviandoMensajeException(
                        $"Status code no exitoso: {response.StatusCode}");
                }
            } catch(Exception ex)
            {
                throw new ErrorEnviandoMensajeException(innerException: ex);
            }
        }

        async Task<HttpResponseMessage> EnviarMensajeAsync(
            HttpClient client,
            string chatPlataformaId,
            Mensaje mensaje)
        {
            if(mensaje is MensajeTexto mensajeTexto)
            {
                return await client.PostAsync(
                    "/Chat",
                    JsonContent.Create(
                        new { ChatId = chatPlataformaId, mensajeTexto.Texto }));
            } else
            {
                throw new NotImplementedException();
            }
        }
    }
}

