using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TvMazeScraper.Controllers.Paging;
using TvMazeScraper.Controllers.Shows.Responses;
using TvMazeScraper.Domain.Models.Shows;
using TvMazeScrapper.Domain.Shows.Commands;
using TvMazeScrapper.Domain.Shows.Queries;

namespace TvMazeScraper.Controllers.Shows
{
    [ApiController]
    [Route("[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ILogger<ShowsController> _logger;
        private readonly IMediator _bus;
        private readonly IMapper _mapper;

        public ShowsController(ILogger<ShowsController> logger, IMediator bus, IMapper mapper)
        {
            _logger = logger;
            _bus = bus;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<List<ShowResponse>> Get([FromQuery] PageRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Test");
            GetShowsWithCastQuery query = new GetShowsWithCastQuery();

            var data = await _bus.Send(query);

            var result = await data.Page<Show, ShowResponse>(_mapper, request, cancellationToken);

            return result;
        }

        [HttpPost]
        public Task Sync()
        {
            var command = new SyncShowsAndCastCommand();

            return _bus.Send(command);
        }
    }
}
