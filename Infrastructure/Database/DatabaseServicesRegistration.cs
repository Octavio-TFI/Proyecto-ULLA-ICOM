using AppServices.Ports;
using Domain.Repositories;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
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
                    options.UseSqlServer(connectionString)
                        .AddInterceptors(new OutboxInterceptor());
                });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IMensajeRepository, MensajeRepository>();

            return services;
        }
    }
}
