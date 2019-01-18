using System.Net;
using TvMaze.Scraper.Core;

namespace TvMaze.Scraper.Sources.Extensions
{
	/// <summary>
	/// Extensions for <see cref="ScrapeResult"/>.
	/// </summary>
	public static class ScrapeResultExtensions
	{
		public static bool IndicatesNotFound<TData>(this ScrapeResult<TData> scrapeResult)
		{
			if (!(scrapeResult is HttpScrapeResult<TData> result))
			{
				return false;
			}

			if (result.IsSuccessful)
			{
				return false;
			}

			if (result.HttpStatusCode == HttpStatusCode.NotFound)
			{
				return true;
			}

			return false;
		}
	}
}