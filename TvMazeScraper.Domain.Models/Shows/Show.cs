using System.Collections.Generic;
using TvMazeScraper.Domain.Models.ActorShows;
using TvMazeScrapper.Domain.Models.Actors;

namespace TvMazeScraper.Domain.Models.Shows
{
    public class Show
    {
        public Show()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Actor> Cast { get; set; }

        public virtual List<ActorShow> CastShows { get; set; }
    }
}
