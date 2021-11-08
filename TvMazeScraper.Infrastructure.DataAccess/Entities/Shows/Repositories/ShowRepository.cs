using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TvMazeScraper.Domain.DataAccess.Contracts.Shows;
using TvMazeScraper.Domain.Models.ActorShows;
using TvMazeScraper.Domain.Models.Shows;
using TvMazeScrapper.Domain.Models.Actors;

namespace TvMazeScraper.Infrastructure.DataAccess.Entities.Shows.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private DbContext _context;
        public ShowRepository(TvMazeDatabaseContext context)
        {
            _context = context;
        }

        public IQueryable<Show> Get()
        {
            return _context.Set<Show>()
                .Include(x => x.Cast)
                .OrderBy(s => s.Id);
        }

        public async Task Merge(IEnumerable<Show> shows, CancellationToken cancellationToken)
        {
            Dictionary<int, List<int>> existingShows = await _context.Set<Show>()
                .Include(x => x.CastShows)
                .ToDictionaryAsync<Show, int, List<int>>(x => x.Id, x => x.CastShows.Select(c => c.ActorId).ToList(), cancellationToken);

            // add shows that dont exist in db and update shows that already exist
            await _context.BulkInsertAsync(shows.Where(x => !existingShows.ContainsKey(x.Id)), cancellationToken);
            await _context.BulkUpdateAsync(shows.Where(x => existingShows.ContainsKey(x.Id)), cancellationToken);

            await MergeCast(shows, cancellationToken);
            await MergeActorCast(shows, existingShows, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task MergeCast(IEnumerable<Show> shows, CancellationToken cancellationToken)
        {
            List<int> castIds = shows.SelectMany(x => x.Cast.Select(x => x.Id)).Distinct().ToList();

            List<int> existingCastIds = await _context.Set<Actor>()
                .Where(x => castIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            List<Actor> cast = shows.SelectMany(x => x.Cast)
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .ToList();
            // add actors that dont exist in db and update actors that already exist
            await _context.BulkInsertAsync(cast.Where(x => !existingCastIds.Contains(x.Id)), cancellationToken);
            await _context.BulkUpdateAsync(cast.Where(x => existingCastIds.Contains(x.Id)), cancellationToken);
        }

        private async Task MergeActorCast(IEnumerable<Show> shows, Dictionary<int, List<int>> existingShowsAndCast, CancellationToken cancellationToken)
        {
            foreach (var show in shows)
            {
                List<int> actorsInTheShow = show.Cast.Select(x => x.Id).ToList();

                await _context.Set<ActorShow>()
                    .Where(x => x.ShowId == show.Id && !actorsInTheShow.Contains(x.ActorId))
                    .DeleteFromQueryAsync(cancellationToken);

                List<int> actorsAlreadyInTheShow = existingShowsAndCast.GetValueOrDefault(show.Id, new List<int>());
                List<ActorShow> actorShowsToAdd = actorsInTheShow.Except(actorsAlreadyInTheShow)
                   .Select(x => new ActorShow() { ActorId = x, ShowId = show.Id })
                   .ToList();

                await _context.BulkInsertAsync(actorShowsToAdd, cancellationToken);
            }
        }

    }
}
