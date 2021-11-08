using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TvMazeScraper.Domain.Models.Shows;
using TvMazeScraper.Domain.Services.Contracts.Shows;
using TvMazeScraper.Infrasctructure.Services.Actors.Dto;
using TvMazeScraper.Infrasctructure.Services.Shows.Dto;
using TvMazeScrapper.Domain.Models.Actors;

namespace TvMazeScraper.Infrasctructure.Services.Shows
{
    public class ShowScraper : IShowScraper
    {
        private readonly HttpClient _client;
        private readonly Uri _baseUri;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private const int PageSize = 250;

        public ShowScraper(HttpClient httpClient, ScrapingConfiguration scrapingConfiguration, JsonSerializerOptions serializerOptions)
        {
            _client = httpClient;
            _baseUri = new Uri(scrapingConfiguration.BaseUri);
            _jsonSerializerOptions = serializerOptions;
        }

        public async IAsyncEnumerable<List<Show>> GetNextShows
        (
            int maxBatch = 250,
            DateTime? lastSuccessfulUpdate = null,
            int scrapingLastSuccessfulShowId = 0,
            [EnumeratorCancellation] CancellationToken cancellationToken = default
        )
        {
            int page = scrapingLastSuccessfulShowId / PageSize;
            long lastSuccessfulUpdateInUnixTimestamp = ConvertDatetimeToUnixTimeStamp(lastSuccessfulUpdate);
            while (true)
            {
                HttpResponseMessage response = await _client.GetAsync(GetPagedShowUrl(page), cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    break;
                }

                List<ShowDto> shows = await Deserialize<ShowDto>(response, cancellationToken);
                shows = shows.Where(x => x.Updated > lastSuccessfulUpdateInUnixTimestamp && x.Id > scrapingLastSuccessfulShowId).ToList();

                for (int i = 0; i < shows.Count; i += maxBatch)
                {
                    List<ShowDto> batch = shows.Skip(i).Take(maxBatch).ToList();
                    if (batch.Any())
                    {
                        yield return await AddCastToShows(batch, cancellationToken);
                    }
                }

                page++;
            }

        }

        public static long ConvertDatetimeToUnixTimeStamp(DateTime? date)
        {
            if (!date.HasValue)
            {
                return 0L;
            }

            var dateTimeOffset = new DateTimeOffset(date.Value);
            var unixDateTime = dateTimeOffset.ToUnixTimeSeconds();
            return unixDateTime;
        }

        private async Task<List<Show>> AddCastToShows(List<ShowDto> shows, CancellationToken cancellationToken)
        {
            List<Show> showsWithActors = new List<Show>();

            foreach (var show in shows)
            {
                List<Actor> cast = (await Get<CastDto>(GetCastUrl(show.Id), cancellationToken))
                    .Select(x => new Actor()
                    {
                        Id = x.Person.Id,
                        Name = x.Person.Name,
                        Birthday = x.Person.Birthday == null ? (DateTime?)null : DateTime.Parse(x.Person.Birthday)
                    }).ToList();

                Show showWithActor = new Show()
                {
                    Id = show.Id,
                    Name = show.Name,
                    Cast = cast
                };

                showsWithActors.Add(showWithActor);
            }

            return showsWithActors;
        }

        private Uri GetPagedShowUrl(int page)
        {
            return new Uri(_baseUri, $"shows?page={page}");
        }

        private Uri GetCastUrl(int showId)
        {
            return new Uri(_baseUri, $"shows/{showId}/cast");
        }


        private async Task<List<T>> Deserialize<T>(HttpResponseMessage httpResponse, CancellationToken cancellationToken) where T : class
        {
            Stream contentStream = await httpResponse.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<T>>(contentStream, _jsonSerializerOptions, cancellationToken);
        }

        private async Task<List<T>> Get<T>(Uri uri, CancellationToken cancellationToken) where T : class
        {
            var response = await _client.GetAsync(uri, cancellationToken);

            return await Deserialize<T>(response, cancellationToken);
        }

    }
}
