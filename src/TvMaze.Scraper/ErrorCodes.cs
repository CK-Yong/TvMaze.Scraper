using TvMaze.Scraper.Core;

namespace TvMaze.Scraper
{
	internal static class ScrapeResultExtensions
	{
		internal static bool IndicatesNotFound<TData>(this ScrapeResult<TData> result)
		{
			return result.ErrorCode == 404;
		}
	}
}
