namespace TvMaze.Scraper.WebHost.Controllers.Paging
{
	public class PagingMetadata
	{
		public PagingMetadata(int page, int pageSize, int totalNumberItems)
		{
			TotalItems = totalNumberItems;
			PageSize = pageSize;

			int totalPages = totalNumberItems / pageSize;
			bool isCleanDivision = totalNumberItems % pageSize == 0;
			TotalPages = isCleanDivision ? totalPages : totalPages + 1;
			PageNumber = page;
		}

		public int TotalItems { get; }
		public int PageNumber { get; }
		public int PageSize { get; }
		public int TotalPages { get; }
	}
}