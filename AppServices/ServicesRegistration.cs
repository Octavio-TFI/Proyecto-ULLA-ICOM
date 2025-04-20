using AppServices.Abstractions;
using AppServices.ConsultasProcessing;
using AppServices.DocumentProcessing;
using AppServices.Ports;
using Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ude;

namespace AppServices
{
    public static class ServicesRegistration
    {
        /// <summary>
        /// Registra los servicios de la capa de aplicación
        /// </summary>
        /// <param name="services">Collección de servicios</param>
        /// <returns>
        /// Collección de servicios que incluye los servicios de la capa de
        /// aplicación
        /// </returns>
        public static IServiceCollection AddAppServices(
            this IServiceCollection services)
        {
            services.AddSingleton<Func<ICharsetDetector>>(
                x => () => new CharsetDetector());

            // Register the code pages encoding provider to support Windows-1252 and other encodings
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Procesamiento documentos
            services.AddHostedService<DocumentProcessorService>();
            services.AddKeyedScoped<IDocumentProcessor, PdfProcessor>(".pdf");
            services.AddKeyedScoped<IDocumentProcessor, HtmlProcessor>(".htm");
            services.AddKeyedScoped<IDocumentProcessor, HtmlProcessor>(".html");
            services.AddKeyedScoped<IDocumentProcessor, MarkdownProcessor>(
                ".md");

            // Procesamiento de consultas
            services.AddHostedService<ConsultasProcesorService>();

            services.AddScoped<IRecibidorMensajes, RecibidorMensajes>();
            services.AddScoped<ICalificadorMensajes, CalificadorMensajes>();

            services.AddSingleton<Func<string, IClient>>(
                services => (string plataforma) =>
                {
                    return services.GetRequiredKeyedService<IClient>(plataforma);
                });

            return services.AddMediatR(
                c => c.RegisterServicesFromAssembly(
                    Assembly.GetExecutingAssembly()));
        }
    }
}
