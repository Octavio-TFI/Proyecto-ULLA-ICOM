﻿using Domain.Entities;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Abstractions
{
    internal interface IChatHistoryFactory
    {
        /// <summary>
        /// Crea un ChatHistory a partir de una lista de mensajes.
        /// </summary>
        /// <param name="mensajes">Lista de mensajes</param>
        /// <returns>ChatHistory con mensajes</returns>
        Task<ChatHistory> CreateAsync(List<Mensaje> mensajes);
    }
}
