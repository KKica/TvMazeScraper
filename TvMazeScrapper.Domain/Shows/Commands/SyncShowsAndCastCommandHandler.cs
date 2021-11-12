using MediatR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TvMazeScraper.Domain.DataAccess.Contracts.Shows;
using TvMazeScraper.Domain.Models.Shows;
using TvMazeScraper.Domain.Services.Contracts;
using TvMazeScraper.Domain.Services.Contracts.Shows;
using TvMazeScrapper.Domain.Exceptions;

namespace TvMazeScrapper.Domain.Shows.Commands
{
    public class SyncShowsAndCastCommandHandler : AsyncRequestHandler<SyncShowsAndCastCommand>
    {
        private readonly IShowScraper _showScraper;
        private readonly IShowRepository _showRepository;
        private readonly IPersistentConfiguration _configuration;
        private readonly ILogger _logger;

        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        public const string ScrapingLastSuccessfulSyncKey = "ScrapingLastSuccessfulSync";
        public const string ScrapingLastSuccessfulShowIdKey = "ScrapingLastSuccessfulShowId";
        public const string ScrapingNextSuccessfulSyncTimeKey = "ScrapingNextSuccessfulSyncTime";

        public SyncShowsAndCastCommandHandler(
            IShowScraper showScraper,
            IShowRepository showRepository,
            IPersistentConfiguration configuration,
            ILogger logger
        )
        {
            _showScraper = showScraper;
            _showRepository = showRepository;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task Handle(SyncShowsAndCastCommand request, CancellationToken cancellationToken)
        {
            if (!await _semaphoreSlim.WaitAsync(0, cancellationToken))
            {
                throw new OperationInProgressException("A show scraping operation is already in progress");
            }

            DateTime? lastUpdated = _configuration.GetAppSetting<DateTime?>(ScrapingLastSuccessfulSyncKey);
            DateTime syncStartTime = DateTime.UtcNow;
            int? scrapingLastSuccessfulShowId = _configuration.GetAppSetting<int>(ScrapingLastSuccessfulShowIdKey);

            // this is a fresh batch. The next run, only the shows that are updated after this time will be taken into consideration
            if (scrapingLastSuccessfulShowId == 0)
            {
                _configuration.AddOrUpdateAppSetting<DateTime?>(
                    ScrapingNextSuccessfulSyncTimeKey,
                    syncStartTime
                );
            }
            try
            {
                await foreach (var batch in _showScraper.GetNextShows(10, lastUpdated, scrapingLastSuccessfulShowId ?? 0, cancellationToken))
                {
                    await HandleBatch(scrapingLastSuccessfulShowId, batch, cancellationToken);
                }

                // all the shows have been updated. We set the ScrapingLastSuccessfulSync to the time the updates started from the beginning
                _configuration.AddOrUpdateAppSetting<DateTime?>(
                        ScrapingLastSuccessfulSyncKey,
                        _configuration.GetAppSetting<DateTime?>(ScrapingNextSuccessfulSyncTimeKey) ?? syncStartTime
                    );
                _configuration.AddOrUpdateAppSetting<int?>(
                    ScrapingLastSuccessfulShowIdKey,
                    0
                );
            }
            finally
            {
                _semaphoreSlim.Release();
            }

        }

        private async Task HandleBatch(int? scrapingLastSuccessfulShowId, List<Show> shows, CancellationToken cancellationToken)
        {
            _logger.Information($"Merging shows with id: {string.Join(",", shows.Select(x => x.Id).ToList())}");
            await _showRepository.Merge(shows, cancellationToken);

            scrapingLastSuccessfulShowId = shows.Last()?.Id;
            if (scrapingLastSuccessfulShowId.HasValue)
            {
                _configuration.AddOrUpdateAppSetting<int?>(
                    ScrapingLastSuccessfulShowIdKey,
                    scrapingLastSuccessfulShowId.Value

                );
            }
        }
    }
}
