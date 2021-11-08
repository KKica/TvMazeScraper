using Microsoft.Extensions.DependencyInjection;
using TvMazeScraper.Middlewares;
using TvMazeScraper.Middlewares.Context;
using TvMazeScraper.Middlewares.ErrorHandling;

namespace TvMazeScraper.ServiceCollectionExtensions
{
    public static class MiddlewareRegistrations
    {
        public static void AddMiddlewares(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddContext();

            serviceCollection.AddTransient<CorrelationHeadersMiddleware>();
            serviceCollection.AddTransient<ErrorHandlingMiddleware>();
            serviceCollection.AddTransient<LogEnrichmentMiddleware>();
        }

        private static void AddContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<Context>();
            serviceCollection.AddScoped<IContext>(sp => sp.GetRequiredService<Context>());
            serviceCollection.AddScoped<IContextInitializer>(sp => sp.GetRequiredService<Context>());
        }
    }
}
