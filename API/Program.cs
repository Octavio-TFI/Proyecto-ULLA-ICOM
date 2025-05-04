using AppServices;
using Controllers;
using Domain;
using Infrastructure.Clients;
using Infrastructure.Database;
using Infrastructure.FileSystem;
using Infrastructure.LLM;
using Infrastructure.Outbox;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var controllersAssembly = Assembly.GetAssembly(typeof(TestController)) ??
    throw new Exception("Assembly de Controllers no econtrada");

builder.Services.AddControllers().AddApplicationPart(controllersAssembly);
builder.Services.AddAppServices();
builder.Services.AddDomainServices();

string connectionString = builder.Configuration.GetConnectionString("Default") ??
    throw new Exception("Connection string no configurado");

builder.Services.AddClientServices();
builder.Services.AddDatabaseServices(connectionString);
builder.Services.AddOutboxServices();
builder.Services.AddLLMServices();
builder.Services.AddFileManagerServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Para poder correr como servicio de Windows
if (builder.Environment.IsProduction())
{
    Directory.SetCurrentDirectory(AppContext.BaseDirectory);
    builder.Services.AddWindowsService();

    builder.WebHost
        .ConfigureKestrel(
            options =>
            {
                options.ListenAnyIP(5000);
            });
}

var app = builder.Build();

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

public partial class APIProgram
{
}