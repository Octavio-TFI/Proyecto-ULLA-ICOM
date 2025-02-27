using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IAgentBuilder
    {
        /// <summary>
        /// Añade una herramienta al agente
        /// </summary>
        /// <typeparam name="T">Tipo de la herramienta</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        IAgentBuilder AddTool<T>(string name);

        /// <summary>
        /// Establece el schema a utilizar para la respuesta
        /// </summary>
        /// <param name="schema">Schema de la respuesta</param>
        /// <returns>AgentBuilder</returns>
        IAgentBuilder SetResponseSchema(Type schema);

        /// <summary>
        /// Establece la temperatura del modelo
        /// </summary>
        /// <param name="temperature">Temperatura</param>
        /// <returns>AgentBuilder</returns>
        IAgentBuilder SetTemperature(double temperature);

        /// <summary>
        /// Crea el agente
        /// </summary>
        /// <param name="name">Nombre del agente</param>
        /// <param name="tipoLLM">Tipo del LLM a utilizar</param>
        /// <returns>Agente configurado</returns>
        IAgent Build(string name, TipoLLM tipoLLM);
    }
}
