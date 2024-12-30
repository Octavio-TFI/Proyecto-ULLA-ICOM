using AppServices;
using Controllers;
using Domain;
using Domain.Repositories;
using Infrastructure.Database;
using Infrastructure.LLM;
using Infrastructure.Outbox;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var controllersAssembly = Assembly.GetAssembly(typeof(TestController)) ??
    throw new Exception("Assembly de Controllers no econtrada");

builder.Services.AddControllers().AddApplicationPart(controllersAssembly);
builder.Services.AddAppServices();
builder.Services.AddDomainServices();

string connectionString = builder.Configuration.GetConnectionString("Chat") ??
    throw new Exception("Connection string no configurado");

builder.Services.AddDatabaseServices(connectionString);
builder.Services.AddOutboxServices();
builder.Services.AddLLMServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var consultas = await app.Services.CreateScope().ServiceProvider
    .GetRequiredService<IConsultaRepository>()
    .GetConsultasSimilaresAsync(new ReadOnlyMemory<float>([1,1,1]));

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
