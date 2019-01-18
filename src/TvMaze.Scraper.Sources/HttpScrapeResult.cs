using System.Net;
using TvMaze.Scraper.Core;

namespace TvMaze.Scraper.Sources
{
	internal class HttpScrapeResult<TData> : ScrapeResult<TData>
	{
		public readonly HttpStatusCode HttpStatusCode;

		public HttpScrapeResult(TData data)
			: base(data)
		{
		}

		public HttpScrapeResult(HttpStatusCode httpStatusCode, string errorMessage)
			: base(errorMessage)

		{
			this.HttpStatusCode = httpStatusCode;
		}

		/// <summary>
		/// Creates a faulted <see cref="ScrapeResult{T}" /> that throws a <see cref="ScrapeDataNotAccessibleException" /> with the given <param name="errorMessage">error message</param> when the data is accessed.
		/// </summary>
		/// <param name="httpStatusCode">The HTTP status code.</param>
		/// <param name="errorMessage">The error message.</param>
		/// <returns>
		/// An initialized <see cref="ScrapeResult{T}" /> that is faulted.
		/// </returns>
		public static ScrapeResult<TData> CreateError(HttpStatusCode httpStatusCode, string errorMessage)
		{
			return new HttpScrapeResult<TData>(httpStatusCode, errorMessage);
		}
	}
}