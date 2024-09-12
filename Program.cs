using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.HuggingFace.Downloader;
using LangChain.Providers.LLamaSharp;
using LangChain.Splitters.Text;
using SoporteLLM.Abstractions;
using SoporteLLM.Business;
using SoporteLLM.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IChatModelFactory, ChatModelFactory>();
builder.Services.AddEmbeddingModel(builder.Configuration);
builder.Services.AddVectorDatabase("vectors.db");
builder.Services.AddSingleton<FileLoader>();
builder.Services.AddSingleton<IEmbeddingService, MesaDeAyudaEmbeddingsService>();
builder.Services.AddSingleton<IDataExtractor, MesaDeAyudaExtractor>();
builder.Services.AddSingleton<IDocumentSplitter, MesaDeAyudaDocumentSplitter>();

var app = builder.Build();

// Crear embeddings
await Task.WhenAll(
    app.Services
        .GetServices<IEmbeddingService>()
        .ToList()
        .Select(x => x.CreateEmbeddingsAsync()));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
