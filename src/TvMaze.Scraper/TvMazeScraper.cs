using System.Threading;
using System.Threading.Tasks;
using TvMaze.Scraper.Core;

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
		/// Retrieves a single show by its id from the <see cref="TvMaze.Scraper.Core.ISource{T}"/> and persists it in the <see cref="TvMaze.Scraper.Core.IRepository{T}"/>. If the scrape fails, returns a faulty result.
		/// </summary>
		public async Task<ScrapeResult<TvShow>> ScrapeAsync(int id, CancellationToken cancellationToken)
		{
			ScrapeResult<TvShow> scraped = await _showSource.GetByIdAsync(id, cancellationToken);

			if (scraped.IsSuccessful)
			{
				await _showRepository.SaveAsync(scraped.Data, cancellationToken);
			}

			return scraped;
		}
	}
}