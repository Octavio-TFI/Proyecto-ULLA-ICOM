using AppServices.Ports;
using Domain;
using Domain.Repositories;
using Infrastructure.Database.Repositories;
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
            // Como SQL Server todavia no soporta Vector Search
            // Se utiliza SQLite
            services.AddDbContext<ChatContext>(
                options =>
                {
                    options.UseSqlite(connectionString)
                        .AddInterceptors(
                            new OutboxInterceptor(),
                            new SQLiteExtensionInterceptor());
                });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IMensajeIARepository, MensajeIARepository>();

            services.AddScoped<IConsultaRepository, ConsultaRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            // Mesa de ayuda
            services.AddSingleton<IConsultaDataRepository, ConsultaDataRepository>(
                );

            return services;
        }
    }
}
