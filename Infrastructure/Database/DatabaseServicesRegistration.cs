using AppServices.Ports;
using Domain;
using Domain.Repositories;
using Infrastructure.Database.Chats;
using Infrastructure.Database.Embeddings;
using Infrastructure.Database.MesaDeAyuda;
using Infrastructure.Outbox;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database
{
    public static class DatabaseServicesRegistration
    {
        public static IServiceCollection AddDatabaseServices(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<ChatContext>(
                options =>
                {
                    options.UseSqlServer(
                        connectionString,
                        o => o.MigrationsHistoryTable(
                                tableName: HistoryRepository.DefaultTableName,
                                schema: "Chat"))
                        .AddInterceptors(new OutboxInterceptor());
                });

            services.AddKeyedScoped<IUnitOfWork, UnitOfWork<ChatContext>>(
                Contexts.Chat);
            services.AddScoped<IChatRepository, ChatRepository>();

            // Como SQL Server todavia no soporta Vector Search
            // Se utiliza SQLite
            services.AddDbContext<EmbeddingContext>(
                options =>
                {
                    options.UseSqlite("Data Source=Embeddings.db")
                        .AddInterceptors(
                            new OutboxInterceptor(),
                            new SQLiteExtensionInterceptor());
                });

            services.AddScoped<IConsultaRepository, ConsultaRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddKeyedScoped<IUnitOfWork, UnitOfWork<EmbeddingContext>>(
                Contexts.Embedding);

            // Mesa de ayuda
            services.AddSingleton<IConsultaDataRepository, ConsultaDataRepository>(
                );

            return services;
        }
    }
}
