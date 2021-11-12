using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Text.Json;
using TvMazeScraper.Domain.Services.Contracts;
using TvMazeScraper.Domain.Services.Contracts.Shows;
using TvMazeScraper.Infrasctructure.Services;
using TvMazeScraper.Infrasctructure.Services.Shows;

namespace TvMazeScraper.ServiceCollectionExtensions
{
    public static class InfrastructureServicesRegistrations
    {
        public static void RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new ScrapingConfiguration() { BaseUri = configuration["ScrapingBaseUrl"] });
            services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

            services.AddSingleton<IPersistentConfiguration>(
                serviceProvider => new PersistentConfiguration(configuration, serviceProvider.GetRequiredService<ILogger>())
            );
            services.AddSingleton<IShowScraper, ShowScraper>();
        }
    }
}
