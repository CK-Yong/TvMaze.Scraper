using System;

namespace TvMaze.Scraper.WebHost.Controllers.Paging
{
	public class Link
	{
		public Link(Uri href, string rel, string method)
		{
			Href = href;
			Rel = rel;
			Method = method;
		}

		public Uri Href { get; }
		public string Rel { get; }
		public string Method { get; }
	}
}