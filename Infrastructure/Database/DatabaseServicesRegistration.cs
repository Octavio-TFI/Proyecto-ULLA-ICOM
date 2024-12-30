using AppServices.Ports;
using Domain;
using Domain.Repositories;
using Infrastructure.Database.Chats;
using Infrastructure.Database.Embeddings;
using Infrastructure.Outbox;
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
            services.AddScoped<IMensajeRepository, MensajeRepository>();

            services.AddDbContext<EmbeddingContext>(
                options =>
                {
                    options.UseSqlServer(
                        connectionString,
                        o => o.UseVectorSearch()
                                .MigrationsHistoryTable(
                                    tableName: HistoryRepository.DefaultTableName,
                                    schema: "Embedding"))
                        .AddInterceptors(new OutboxInterceptor());
                });

            services.AddKeyedScoped<IUnitOfWork, UnitOfWork<EmbeddingContext>>(
                Contexts.Embedding);

            return services;
        }
    }
}
