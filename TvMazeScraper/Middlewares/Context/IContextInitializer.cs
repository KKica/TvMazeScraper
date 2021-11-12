using System;

namespace TvMazeScraper.Middlewares.Context
{
    public interface IContextInitializer
    {
        void Initialize(Guid correlationId);
    }
}
