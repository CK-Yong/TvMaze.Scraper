using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.Core.Domain;
using TvMaze.Scraper.WebHost.Controllers.Paging;

namespace TvMaze.Scraper.WebHost.Controllers
{
	[Route("api/shows")]
	[ApiController]
	public class TvShowController : ControllerBase
	{
		private readonly IRepository<TvShow> _showRepository;
		private static int _totalItems = 0;

		public TvShowController(IRepository<TvShow> showRepository)
		{
			_showRepository = showRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetListAsync(CancellationToken cancellationToken, int page = 0, int pageSize = 0)
		{
			if (_totalItems == 0)
			{
				_totalItems = await _showRepository.GetTotalItemsAsync(cancellationToken);
			}

			if (pageSize <= default(int))
			{
				pageSize = 10;
			}

			if (page <= default(int))
			{
				page = 1;
			}

			var startIndex = (page - 1) * pageSize;
			var shows = await _showRepository.GetMultipleAsync(startIndex, pageSize, cancellationToken);
			foreach (var show in shows)
			{
				show.Cast = show.Cast.OrderByDescending(cast => cast.Birthday);
			}

			var paginatedResult = new PaginatedResult<TvShow>(shows, page, pageSize, _totalItems, HttpContext.Request.GetUri(), nameof(page), nameof(pageSize), HttpContext.Request.Method);

			return Ok(paginatedResult);
		}
	}
}