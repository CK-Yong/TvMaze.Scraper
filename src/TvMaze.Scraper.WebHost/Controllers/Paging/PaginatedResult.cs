using System;
using System.Collections.Generic;
using System.Net.Http;

namespace TvMaze.Scraper.WebHost.Controllers.Paging
{
	public class PaginatedResult<TData>
	{
		public PagingMetadata Paging { get; }
		public PageLinks Links { get; }
		public IEnumerable<TData> Data { get; }

		public PaginatedResult(IEnumerable<TData> data, int page, int pageSize, int totalNumberItems, Uri originalUri, string pageName, string pageSizeName, string method)
		{
			Data = data;
			Paging = new PagingMetadata(page, pageSize, totalNumberItems);

			var query = originalUri.ParseQueryString();
			query[pageName] = (page + 1).ToString();
			query[pageSizeName] = pageSize.ToString();

			var nextPageUri = new UriBuilder(originalUri)
			{
				Query = query.ToString()
			}.Uri;
			Links = new PageLinks(originalUri, nextPageUri, method);
		}
	}
}