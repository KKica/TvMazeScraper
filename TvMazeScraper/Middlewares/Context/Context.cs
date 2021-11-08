using System;
using System.Collections.Generic;

namespace TvMazeScraper.Middlewares.Context
{
    public class Context : IContext, IContextInitializer
    {
        public Guid CorrelationId { get; private set; }

        public void Initialize(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                [nameof(CorrelationId)] = CorrelationId
            };
        }
    }
}
