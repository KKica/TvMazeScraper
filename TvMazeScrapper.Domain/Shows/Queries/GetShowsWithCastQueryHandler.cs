using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TvMazeScraper.Domain.DataAccess.Contracts.Shows;
using TvMazeScraper.Domain.Models.Shows;

namespace TvMazeScrapper.Domain.Shows.Queries
{
    public class GetShowsWithCastQueryHandler : IRequestHandler<GetShowsWithCastQuery, IQueryable<Show>>
    {
        private readonly IShowRepository _showRepository;

        public GetShowsWithCastQueryHandler(IShowRepository showRepository)
        {
            _showRepository = showRepository;
        }
        public Task<IQueryable<Show>> Handle(GetShowsWithCastQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_showRepository.Get());
        }
    }
}
