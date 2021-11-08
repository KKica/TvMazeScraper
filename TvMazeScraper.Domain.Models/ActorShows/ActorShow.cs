using TvMazeScraper.Domain.Models.Shows;
using TvMazeScrapper.Domain.Models.Actors;

namespace TvMazeScraper.Domain.Models.ActorShows
{
    public class ActorShow
    {
        public int ActorId { get; set; }
        public int ShowId { get; set; }
        public Actor Actor { get; set; }
        public Show Show { get; set; }
    }
}
