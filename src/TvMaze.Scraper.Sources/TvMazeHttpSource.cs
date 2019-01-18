using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.Sources.Extensions;

namespace TvMaze.Scraper.Sources
{
	public class TvMazeHttpSource : ISource<TvShow>
	{
		private readonly HttpClient _httpClient;
		private readonly JsonSerializer _serializer;

		/// <summary>
		/// Initializes a new instance of the <see cref="TvMazeHttpSource"/> class.
		/// </summary>
		/// <param name="httpClient">The HTTP client.</param>
		public TvMazeHttpSource(HttpClient httpClient)
		{
			_httpClient = httpClient;
			_serializer = new JsonSerializer();
		}

		/// <summary>
		/// Gets the entity of type <see cref="!:T" /> by identifier asynchronously.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<ScrapeResult<TvShow>> GetByIdAsync(int id, CancellationToken cancellationToken)
		{
			var getTvShow = GetTvShowAsync(id, cancellationToken);
			var getCast = GetCastAsync(id, cancellationToken);
			await Task.WhenAll(getTvShow, getCast);
			var tvShowResult = await getTvShow;
			var castResult = await getCast;

			if (!tvShowResult.IsSuccessful)
			{
				return tvShowResult;
			}

			if (!castResult.IsSuccessful)
			{
				return ScrapeResult<TvShow>.CreateError(castResult.ErrorMessage);
			}

			var tvShow = tvShowResult.Data;
			var cast = castResult.Data;
			tvShow.Cast = cast.Select(c => c.MapToCore());

			return new ScrapeResult<TvShow>(tvShow);
		}

		private async Task<ScrapeResult<TvShow>> GetTvShowAsync(int id, CancellationToken cancellationToken)
		{
			var getTvShow = _httpClient.GetAsync($"shows/{id}", cancellationToken);

			var tvShowResponse = await getTvShow;

			if (tvShowResponse.StatusCode == HttpStatusCode.NotFound)
			{
				return HttpScrapeResult<TvShow>.CreateError(tvShowResponse.StatusCode, $"Could not get the show with ID {id}, the remote API returned {((int)tvShowResponse.StatusCode)} - {tvShowResponse.StatusCode}");
			}

			if (!tvShowResponse.IsSuccessStatusCode)
			{
				return ScrapeResult<TvShow>
					.CreateError($"Could not get the show with ID {id}, the remote API returned {((int)tvShowResponse.StatusCode)} - {tvShowResponse.StatusCode}");
			}

			var tvShow = await DeserializeAsync<TvShow>(tvShowResponse.Content);
			return new ScrapeResult<TvShow>(tvShow);
		}

		private async Task<ScrapeResult<IEnumerable<Models.CastMember>>> GetCastAsync(int id, CancellationToken cancellationToken)
		{
			var castResponse = await _httpClient.GetAsync($"shows/{id}/cast", cancellationToken);

			if (castResponse.StatusCode == HttpStatusCode.NotFound)
			{
				return HttpScrapeResult<IEnumerable<Models.CastMember>>.CreateError(castResponse.StatusCode, $"Could not get the cast for the show with ID {id}, the remote API returned {((int)castResponse.StatusCode)} - {castResponse.StatusCode}");
			}

			if (!castResponse.IsSuccessStatusCode)
			{
				return ScrapeResult<IEnumerable<Models.CastMember>>
					.CreateError($"Could not get the cast for the show with ID {id}, the remote API returned  {((int)castResponse.StatusCode)} - {castResponse.StatusCode}");
			}

			var cast = await DeserializeAsync<IEnumerable<Models.CastMember>>(castResponse.Content);
			return new ScrapeResult<IEnumerable<Models.CastMember>>(cast);
		}

		private async Task<T> DeserializeAsync<T>(HttpContent content)
		{
			var stream = await content.ReadAsStreamAsync();
			using (var textReader = new StreamReader(stream))
			using (var jsonReader = new JsonTextReader(textReader))
			{
				return _serializer.Deserialize<T>(jsonReader);
			}
		}
	}
}