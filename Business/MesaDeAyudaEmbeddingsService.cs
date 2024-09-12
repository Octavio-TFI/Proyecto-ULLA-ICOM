using LangChain.Databases;
using LangChain.Databases.Sqlite;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.Google.Extensions;
using LangChain.Providers.Ollama;
using LangChain.Splitters.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using OpenAI.Constants;
using SoporteLLM.Abstractions;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SoporteLLM.Business
{
    public class MesaDeAyudaEmbeddingsService(
        IVectorDatabase vectorDatabase,
        IEmbeddingModel embeddingModel,
        IDataExtractor dataExtractor,
        IDocumentSplitter documentSplitter,
        FileLoader fileLoader) : IEmbeddingService
    {
        const string collectionName = "MesaDeAyuda";
        const string mesaDeAyudaFile = ".\\MDA.xml";
        const string mesaDeAyudaExtracted = ".\\ExtractedMDA.xml";

        public async Task CreateEmbeddingsAsync()
        {
            if (!File.Exists(mesaDeAyudaExtracted))
            {
                Console.WriteLine("Parseando datos de mesa de ayuda...");

                await dataExtractor.ExtractDataAsync(
                    mesaDeAyudaFile,
                    mesaDeAyudaExtracted);
            }

            var vectorCollection = await vectorDatabase.GetOrCreateCollectionAsync(
                collectionName,
                dimensions: 768) // nomic-embed-text es 768
                .ConfigureAwait(false);

            var documents = await fileLoader.LoadAsync(
                DataSource.FromPath(mesaDeAyudaExtracted))
                .ConfigureAwait(false);

            var splittedDocs = documentSplitter.SplitDocument(documents);

            Console.WriteLine("Creando embeddings de mesa de ayuda...");

            long? startTime = null;
            int? vectorStart = null;

            // Ir de a uno
            for (int i = 0; i < splittedDocs.Count; i++)
            {
                bool created = await CreateEmbeddings(
                    vectorCollection,
                    splittedDocs[i]);

                if (!created)
                {
                    continue;
                }

                // Para calcular tiempo restante
                startTime ??= DateTime.Now.Ticks;
                vectorStart ??= i;

                // Calcular tiempo restante
                long tiempoPasado = DateTime.Now.Ticks - startTime.Value;
                int embeddingsCalculados = (i + 1) - vectorStart.Value;
                int embeddingsRestantes = splittedDocs.Count - (i + 1);

                var tiempoRestante = (tiempoPasado / embeddingsCalculados) *
                    embeddingsRestantes;

                Console.WriteLine(
                    $"{i + 1}/{splittedDocs.Count} Tiempo Restante: {TimeSpan.FromTicks(tiempoRestante)}");
            }

            Console.WriteLine("Embeddings de mesa de ayuda creados");
        }

        /// <summary>
        /// Genera embeddings para este documento y los inserta en la base de
        /// datos
        /// </summary>
        /// <param name="vectorCollection">Collecion de vectores</param>
        /// <param name="doc">Documento a generar Embedding</param>
        /// <returns>Verdadero si no existia en la base de datos</returns>
        private async Task<bool> CreateEmbeddings(
            IVectorCollection vectorCollection,
            Document doc)
        {
            var id = doc.Metadata["Id"].ToString() ??
                throw new Exception("Id de documento no puede ser null");
            var text = doc.PageContent;
            var metada = doc.Metadata.ExceptBy(["Id"], x => x.Key);

            // Si ya existe en la base de datos, no generar embedding
            if (await vectorCollection.GetAsync(id) is not null)
            {
                return false;
            }

            var emmbedding = await embeddingModel.CreateEmbeddingsAsync(
                new EmbeddingRequest
                {
                    Strings = [text]
                });

            var vector = new Vector
            {
                Id = id,
                Text = text,
                Embedding = emmbedding.Values.First(),
                Metadata = doc.Metadata
            };

            await vectorCollection.AddAsync([vector]);

            return true;
        }
    }
}
