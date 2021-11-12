using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using TvMazeScraper.Domain.Services.Contracts.Shows;
using TvMazeScraper.Infrasctructure.Services.Shows;

namespace TvMazeScraper.ServiceCollectionExtensions
{
    public static class HttpClientRegistrations
    {
        public static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IShowScraper, ShowScraper>()
                .AddPolicyHandler(
                    (serviceProvider, request) => HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .OrResult(response => response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                        .WaitAndRetryAsync(
                            configuration.GetValue<int>("RetryHttpClient"),
                            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            OnRetry(serviceProvider)
                        )
                );
        }

        private static Action<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context> OnRetry(IServiceProvider serviceProvider)
        {
            return (result, timeSpan, retryCount, context) =>
            {
                var logger = serviceProvider.GetRequiredService<Serilog.ILogger>();
                logger.Information($"An error (HttpCode: {result.Result.StatusCode}) occurred while performing call to " +
                    $"{result.Result.RequestMessage.RequestUri})"
                );
                logger.Information($"Retry count {retryCount})");
            };
        }
    }


}
