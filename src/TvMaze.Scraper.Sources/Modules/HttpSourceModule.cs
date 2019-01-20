using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Autofac;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.Core.Domain;

namespace TvMaze.Scraper.Sources.Modules
{
	/// <summary>
	/// Registers the HTTP implementation of <see cref="ISource{T}"/>.
	/// </summary>
	/// <seealso cref="Autofac.Module" />
	[ExcludeFromCodeCoverage]
	public class HttpSourceModule : Module
	{
		private HttpClient _httpClient = new HttpClient();
		private Uri _baseUri;

		public HttpSourceModule BaseUri(Uri uri)
		{
			_baseUri = uri;
			return this;
		}

		public HttpSourceModule WithHttpClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
			return this;
		}

		protected override void Load(ContainerBuilder builder)
		{
			_httpClient.BaseAddress = _baseUri;
			builder.RegisterInstance(new TvMazeHttpSource(_httpClient))
				.As<ISource<TvShow>>();
		}
	}
}
