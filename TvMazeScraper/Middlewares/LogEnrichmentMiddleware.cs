using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TvMazeScraper.Middlewares.Context;

namespace TvMazeScraper.Middlewares
{
    public class LogEnrichmentMiddleware : IMiddleware
    {
        private readonly IContext _context;
        private readonly ILogger _logger;

        public LogEnrichmentMiddleware(IContext context, ILogger<LogEnrichmentMiddleware> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using var scope = _logger.BeginScope(_context.ToDictionary());

            await next(context);
        }
    }
}
