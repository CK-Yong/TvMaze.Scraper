using System;
using System.Collections;
using System.Collections.Generic;

namespace TvMaze.Scraper.WebHost.Controllers.Paging
{
	public class PageLinks : IEnumerable<Link>
	{
		private readonly Uri _thisPage;
		private readonly Uri _nextPage;
		private readonly string _method;

		/// <summary>
		/// Initializes a new instance of the <see cref="PageLinks"/> class.
		/// </summary>
		/// <param name="thisPage">The URI to this page.</param>
		/// <param name="nextPage">The URI to next page.</param>
		/// <param name="method">The method to use when accessing the URIs.</param>
		public PageLinks(Uri thisPage, Uri nextPage, string method)
		{
			_thisPage = thisPage;
			_nextPage = nextPage;
			_method = method;
		}

		public IEnumerator<Link> GetEnumerator()
		{
			yield return new Link(_thisPage, "self", _method);
			yield return new Link(_nextPage, "nextPage", _method);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}