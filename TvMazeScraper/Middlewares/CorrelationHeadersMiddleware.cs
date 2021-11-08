using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using TvMazeScraper.Middlewares.Context;

namespace TvMazeScraper.Middlewares
{
    public class CorrelationHeadersMiddleware : IMiddleware
    {
        protected readonly IContextInitializer _contextInitializer;
        public CorrelationHeadersMiddleware(IContextInitializer contextInitializer)
        {
            _contextInitializer = contextInitializer;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            IHeaderDictionary requestHeaders = context.Request.Headers;
            IHeaderDictionary responseHeaders = context.Response.Headers;

            Guid correlationId = GetOrCreateCorrelationId(requestHeaders);

            SetCorrelationIdHeader(requestHeaders, correlationId);
            SetCorrelationIdHeader(responseHeaders, correlationId);


            _contextInitializer.Initialize(correlationId);

            await next(context);
        }

        private Guid GetOrCreateCorrelationId(IHeaderDictionary headers)
        {
            string correlationIdHeaderValue = headers
                .SingleOrDefault(h => h.Key == Constants.CorrelationIdHeaderKey).Value
                .FirstOrDefault();

            if (Guid.TryParse(correlationIdHeaderValue, out Guid correlationId))
            {
                return correlationId;
            }

            return Guid.NewGuid();
        }

        private void SetCorrelationIdHeader(IHeaderDictionary headers, Guid correlationId)
        {
            if (headers.ContainsKey(Constants.CorrelationIdHeaderKey))
            {
                headers[Constants.CorrelationIdHeaderKey] = correlationId.ToString();
                return;
            }

            headers.Add(Constants.CorrelationIdHeaderKey, correlationId.ToString());
        }
    }
}
