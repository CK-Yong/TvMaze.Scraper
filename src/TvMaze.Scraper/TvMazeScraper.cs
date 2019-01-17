using System.Threading;
using System.Threading.Tasks;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.Core.Classes;

namespace TvMaze.Scraper
{
	public class TvMazeScraper
	{
		private readonly ISource<TvShow> _showSource;
		private readonly IRepository<TvShow> _showRepository;

		public TvMazeScraper(ISource<TvShow> showSource, IRepository<TvShow> showRepository)
		{
			_showSource = showSource;
			_showRepository = showRepository;
		}

		/// <summary>
		/// Retrieves a single show by its id from the <see cref="TvMaze.Scraper.Core.ISource{T}"/> and persists it in the <see cref="TvMaze.Scraper.Core.IRepository{T}"/>
		/// </summary>
		public async Task ScrapeAsync(int id, CancellationToken cancellationToken)
		{
			var scraped = await _showSource.GetByIdAsync(id, cancellationToken);
			await _showRepository.SaveAsync(scraped, cancellationToken);
		}
	}
}