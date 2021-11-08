using System;
using System.Collections.Generic;

namespace TvMazeScraper.Middlewares.Context
{
    public interface IContext
    {
        Guid CorrelationId { get; }

        IDictionary<string, object> ToDictionary();
    }
}
