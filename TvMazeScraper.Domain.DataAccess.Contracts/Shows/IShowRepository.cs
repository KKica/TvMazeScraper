using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TvMazeScraper.Domain.Models.Shows;

namespace TvMazeScraper.Domain.DataAccess.Contracts.Shows
{
    public interface IShowRepository
    {
        IQueryable<Show> Get();

        Task Merge(IEnumerable<Show> shows, CancellationToken cancellationToken);
    }
}
