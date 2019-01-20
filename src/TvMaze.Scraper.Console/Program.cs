using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.Core.Domain;
using TvMaze.Scraper.Repository;
using TvMaze.Scraper.Repository.Modules;
using TvMaze.Scraper.Sources.Modules;

namespace TvMaze.Scraper.Console
{
	public class Program
	{
		private static int _currentIndex = 1;
		private static int _consecutiveNotFoundCount = 0;

		public static async Task Main(string[] args)
		{
			var builder = new ContainerBuilder();
			builder.RegisterModule(new HttpSourceModule().BaseUri(new Uri("http://api.tvmaze.com/")));
			builder.RegisterModule<MsSqlRepositoryModule>();
			builder.RegisterModule<TvMazeScraperModule>();

			builder.RegisterType<NHibernateTVShowRepository>()
				.AsSelf();

			var container = builder.Build();

			using (var lifeTimeScope = container.BeginLifetimeScope())
			{
				var ctSource = new CancellationTokenSource();

				var scraper = lifeTimeScope.Resolve<TvMazeScraper>();

				var repository = lifeTimeScope.Resolve<NHibernateTVShowRepository>();

				_currentIndex = await repository.GetLastIndexAsync(ctSource.Token);

				System.Console.CancelKeyPress += (sender, eventArgs) => ctSource.Cancel();

				ScrapeResult<TvShow> result;
				do
				{
					result = await scraper.ScrapeAsync(_currentIndex, ctSource.Token);
					if (result.IsSuccessful)
					{
						_consecutiveNotFoundCount = 0;
						System.Console.WriteLine($"Saved {result.Data.Name} (ID {result.Data.Id}) to the persistent storage.");
					}

					System.Console.WriteLine(result.ErrorMessage);

					if (result.ErrorCode == ErrorCode.NotFound)
					{
						_consecutiveNotFoundCount++;
					}

					if (result.ErrorCode == ErrorCode.TooManyRequests)
					{
						System.Console.WriteLine("Retrying after 5 seconds.");
						Thread.Sleep(5000);
						continue;
					}

					_currentIndex++;
				} while (_consecutiveNotFoundCount < 100
				         && !ctSource.Token.IsCancellationRequested);
			}
		}
	}
}