using Domain.Abstractions;
using Domain.Events;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ChatAgregado
{
    public class Chat
        : Entity
    {
        /// <summary>
        /// Id del usuario en la plataforma que envia el mensaje
        /// </summary>
        public required string UsuarioId { get; init; }

        /// <summary>
        /// Id del chat en la plataforma que envia el mensaje
        /// </summary>
        public required string ChatPlataformaId { get; init; }

        /// <summary>
        /// Platforma utilizada para enviar el mensaje
        /// </summary>
        public required string Plataforma { get; init; }

        /// <summary>
        /// Lista de mensajes
        /// </summary>
        public List<Mensaje> Mensajes { get; } = [];

        public Mensaje UltimoMensaje
            => Mensajes.OrderBy(m => m.DateTime).Last();

        public bool UltimoMensajeEsLlamadoHerramienta
            => UltimoMensaje is MensajeLlamadaHerramienta;

        /// <summary>
        /// Añade un mensaje de texto recibido
        /// </summary>
        /// <param name="dateTime">Date Time de cuando se envio el mensaje</param>
        /// <param name="texto">Texto del mensaje</param>
        public virtual Mensaje AñadirMensajeTextoRecibido(
            DateTime dateTime,
            string texto)
        {
            var mensaje = new MensajeTextoUsuario
            {
                Texto = texto,
                DateTime = dateTime
            };

            Mensajes.Add(mensaje);
            Events.Add(new MensajeRecibidoEvent { EntityId = Id });

            return mensaje;
        }

        /// <summary>
        /// Generar un mensaje de respuesta y lo añade a la lista de mensajes
        /// </summary>
        /// <param name="agente">Agente</param>
        /// <returns>Mensaje de respuesta generado</returns>
        public virtual async Task<Mensaje> GenerarMensajeAsync(IAgent agente)
        {
            Mensaje mensaje;

            var agentResult = await agente
                    .GenerarRespuestaAsync(Mensajes)
                .ConfigureAwait(false);

            if (agentResult.FunctionCalls.Count != 0)
            {
                // Por el momento solo soporta un llamado a la vez
                var functionCall = agentResult.FunctionCalls.First();

                mensaje = new MensajeLlamadaHerramienta
                {
                    DateTime = DateTime.Now,
                    PluginName = functionCall.PluginName,
                    FunctionName = functionCall.FunctionName,
                    Argumentos = functionCall.Arguments
                };

                Events.Add(
                    new LlamadaHerramientaGeneradaEvent
                    {
                        EntityId = Id,
                        MensajeLlamadaHerramientaGeneradaId = mensaje.Id
                    });
            }
            else
            {
                mensaje = new MensajeIA
                {
                    DateTime = DateTime.Now,
                    Texto = agentResult.Texto
                };

                Events.Add(
                    new MensajeIAGeneradoEvent
                    {
                        EntityId = Id,
                        MensajeId = mensaje.Id
                    });
            }

            Mensajes.Add(mensaje);

            return mensaje;
        }

        public async Task<MensajeHerramienta> LlamarHerramientaAsyn(
            IAgent agent)
        {
            if (!UltimoMensajeEsLlamadoHerramienta)
            {
                throw new InvalidOperationException(
                    "No se puede llamar herramienta porque ultimo mensaje no es una llamda de herramienta");
            }

            var result = await agent.LlamarHerramientaAsync(
                (UltimoMensaje as MensajeLlamadaHerramienta)!);

            var mensaje = new MensajeHerramienta
            {
                DateTime = DateTime.Now,
                Texto = result.Texto,
            };

            Mensajes.Add(mensaje);
            // Que la herramienta se ejecute es como un mensaje recibido para el LLM
            // Se realiza la misma accion en ambos casos (Generar una respuesta)
            Events.Add(new MensajeRecibidoEvent { EntityId = Id });

            return mensaje;
        }
    }
}
