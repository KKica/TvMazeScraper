using AutoMapper;
using System.Linq;
using TvMazeScraper.Controllers.Shows.Responses;
using TvMazeScraper.Domain.Models.Shows;

namespace TvMazeScraper.Controllers.Shows.Mappers
{
    public class ShowMappingProfile : Profile
    {
        public ShowMappingProfile()
        {
            CreateMap<Show, ShowResponse>()
                .ForMember(
                    x => x.Cast,
                    opts => opts.MapFrom(x => x.Cast.OrderByDescending(c => c.Birthday))
                );
        }
    }
}
