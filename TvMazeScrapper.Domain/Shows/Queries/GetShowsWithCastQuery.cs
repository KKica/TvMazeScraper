using MediatR;
using System.Linq;
using TvMazeScraper.Domain.Models.Shows;

namespace TvMazeScrapper.Domain.Shows.Queries
{
    public class GetShowsWithCastQuery : IRequest<IQueryable<Show>>
    {

    }
}
