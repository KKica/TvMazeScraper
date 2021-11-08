using System.Collections.Generic;
using TvMazeScraper.Controllers.Actors.Responses;

namespace TvMazeScraper.Controllers.Shows.Responses
{
    public class ShowResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ActorResponse> Cast { get; set; }
    }
}
