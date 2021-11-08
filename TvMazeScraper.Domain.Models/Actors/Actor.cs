using System;
using System.Collections.Generic;
using TvMazeScraper.Domain.Models.ActorShows;
using TvMazeScraper.Domain.Models.Shows;

namespace TvMazeScrapper.Domain.Models.Actors
{
    public class Actor
    {
        public Actor()
        {
            Shows = new List<Show>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? Birthday { get; set; }

        public virtual ICollection<Show> Shows { get; set; }

        public virtual List<ActorShow> ActorShows { get; set; }
    }
}
