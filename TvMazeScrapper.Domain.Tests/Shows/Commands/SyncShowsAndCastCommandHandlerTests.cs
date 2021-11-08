using MediatR;
using Moq;
using NUnit.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TvMazeScraper.Domain.DataAccess.Contracts.Shows;
using TvMazeScraper.Domain.Models.Shows;
using TvMazeScraper.Domain.Services.Contracts;
using TvMazeScraper.Domain.Services.Contracts.Shows;
using TvMazeScrapper.Domain.Shows.Commands;
using TvMazeScrapper.Domain.Tests.Common;

namespace TvMazeScrapper.Domain.Tests.Shows.Comands
{
    [TestFixture]
    public class SyncShowsAndCastCommandHandlerTests
    {
        #region Mocks

        private IShowScraper _showScraper;
        private IShowRepository _showRepository;
        private IPersistentConfiguration _configuration;
        private ILogger _logger;

        #endregion
        private IRequestHandler<SyncShowsAndCastCommand> _sut;

        #region Setup

        [SetUp]
        public void Setup()
        {
            _showScraper = Mock.Of<IShowScraper>();
            _showRepository = Mock.Of<IShowRepository>();
            _configuration = Mock.Of<IPersistentConfiguration>();
            _logger = Mock.Of<ILogger>();

            _sut = new SyncShowsAndCastCommandHandler(_showScraper, _showRepository, _configuration, _logger);
        }

        [Test]
        public async Task Handler_gets_last_successful_data()
        {
            SetupNoShows();

            await _sut.Handle(new SyncShowsAndCastCommand(), It.IsAny<CancellationToken>());

            Mock.Get(_configuration)
               .Verify(
                   x => x.GetAppSetting<DateTime?>(SyncShowsAndCastCommandHandler.ScrapingLastSuccessfulSyncKey)
               );
            Mock.Get(_configuration)
               .Verify(
                   x => x.GetAppSetting<int>(SyncShowsAndCastCommandHandler.ScrapingLastSuccessfulShowIdKey)
               );
        }


        [Test]
        public async Task Handler_sets_ScrapingNextSuccessfulSyncTimeKey_when_its_a_fresh_batch()
        {
            SetupNoShows();

            Mock.Get(_configuration)
               .Setup(
                   x => x.GetAppSetting<int>(SyncShowsAndCastCommandHandler.ScrapingLastSuccessfulShowIdKey)
               ).Returns(0);

            await _sut.Handle(new SyncShowsAndCastCommand(), It.IsAny<CancellationToken>());



            Mock.Get(_configuration)
                .Verify(
                    x => x.AddOrUpdateAppSetting<DateTime?>(
                        SyncShowsAndCastCommandHandler.ScrapingNextSuccessfulSyncTimeKey,
                        It.IsAny<DateTime>()
                    ), Times.Once()
                );
        }

        [Test]
        [TestCase(3)]
        [TestCase(10)]
        [TestCase(250)]
        public async Task Handler_doesnt_set_ScrapingNextSuccessfulSyncTimeKey_when_its_not_a_fresh_batch(int lastSuccessfulShowId)
        {
            SetupNoShows();

            Mock.Get(_configuration)
              .Setup(x => x.GetAppSetting<int>(SyncShowsAndCastCommandHandler.ScrapingLastSuccessfulShowIdKey))
              .Returns(lastSuccessfulShowId);


            await _sut.Handle(new SyncShowsAndCastCommand(), It.IsAny<CancellationToken>());


            Mock.Get(_configuration)
                .Verify(
                    x => x.AddOrUpdateAppSetting<DateTime?>(
                        SyncShowsAndCastCommandHandler.ScrapingNextSuccessfulSyncTimeKey,
                        It.IsAny<DateTime?>()
                    ), Times.Never()
                );
        }

        [Test]
        [TestCase(4)]
        [TestCase(42)]
        [TestCase(0)]
        public async Task Handler_gets_shows_and_merges_them(int lastSuccessfulShowSyncId)
        {
            List<Show> firstBatch = EnumerableExtensions.GetMockedList<Show>(3);
            List<Show> secondBatch = EnumerableExtensions.GetMockedList<Show>(2);

            var showsToProccess = new List<List<Show>>() { firstBatch, secondBatch };
            var cancellationToken = new CancellationToken();

            Mock.Get(_configuration)
                .Setup(x => x.GetAppSetting<int>(SyncShowsAndCastCommandHandler.ScrapingLastSuccessfulShowIdKey))
                .Returns(lastSuccessfulShowSyncId);

            Mock.Get(_showScraper)
                .Setup(
                    x => x.GetNextShows(
                        It.IsAny<int>(),
                        It.IsAny<DateTime?>(),
                        lastSuccessfulShowSyncId,
                        It.IsAny<CancellationToken>()
                    )
                ).Returns(showsToProccess.ToAsyncEnumerable());

            await _sut.Handle(new SyncShowsAndCastCommand(), cancellationToken);

            foreach (var batch in showsToProccess)
            {
                Mock.Get(_showRepository)
                    .Verify(x => x.Merge(batch, cancellationToken), Times.Once());
            }

            Mock.Get(_configuration)
                .Verify(
                    x => x.AddOrUpdateAppSetting<int?>(
                        SyncShowsAndCastCommandHandler.ScrapingLastSuccessfulShowIdKey,
                        It.IsAny<int>()
                    ), Times.Exactly(showsToProccess.Count + 1)
                );
        }

        private void SetupNoShows()
        {
            Mock.Get(_showScraper)
                .Setup(
                    x => x.GetNextShows(
                        It.IsAny<int>(),
                        It.IsAny<DateTime?>(),
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()
                    )
                ).Returns(new List<List<Show>>().ToAsyncEnumerable());
        }

        #endregion
    }

}
