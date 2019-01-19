using System;
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
			var getTvShow = GetAsync<TvShow>($"shows/{id}", cancellationToken);
			var getCast = GetAsync<IEnumerable<Models.CastMember>>($"shows/{id}/cast", cancellationToken);
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

		private async Task<ScrapeResult<T>> GetAsync<T>(string resource, CancellationToken cancellationToken)
		{
			var tvShowResponse = await _httpClient.GetAsync(resource, cancellationToken);

			if (!tvShowResponse.IsSuccessStatusCode)
			{
				return GenerateError<T>(tvShowResponse.RequestMessage.RequestUri, tvShowResponse.StatusCode);
			}

			var deserialized = await DeserializeAsync<T>(tvShowResponse.Content);
			return new ScrapeResult<T>(deserialized);
		}

		private ScrapeResult<T> GenerateError<T>(Uri requestUri, HttpStatusCode statusCode)
		{
			return ScrapeResult<T>.CreateError($"Could not retrieve the resource at {requestUri}, the remote API returned {((int) statusCode)} - {statusCode}", (int) statusCode);
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