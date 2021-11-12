using AutoMapper;
using TvMazeScraper.Controllers.Actors.Responses;
using TvMazeScrapper.Domain.Models.Actors;

namespace TvMazeScraper.Controllers.Actors.Mappers
{
    public class ActorMappingProfile : Profile
    {
        public ActorMappingProfile()
        {
            CreateMap<Actor, ActorResponse>()
                .ForMember(
                    x => x.Birthday,
                    opts =>
                    {
                        opts.PreCondition(x => x.Birthday.HasValue);
                        opts.MapFrom(x => x.Birthday.Value.ToString("yyyy-MM-dd"));
                    }
                );
        }
    }
}
