using System;
using System.Collections.Generic;
using System.Threading;
using TvMazeScraper.Domain.Models.Shows;

namespace TvMazeScraper.Domain.Services.Contracts.Shows
{
    public interface IShowScraper
    {
        IAsyncEnumerable<List<Show>> GetNextShows(
            int maxBatch = 250,
            DateTime? lastSuccessfulUpdate = null,
            int scrapingLastSuccessfulShowId = 0,
            CancellationToken cancellationToken = default
        );
    }
}
