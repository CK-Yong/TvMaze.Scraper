using System;

namespace TvMaze.Scraper.Core
{
	/// <summary>
	/// <see cref="Exception"/> that is thrown when the data housed within the <see cref="ScrapeResult{T}"/> is not accessible due to a fault.
	/// </summary>
	/// <seealso cref="System.Exception" />
	public class ScrapeDataNotAccessibleException : Exception
	{
		public ScrapeDataNotAccessibleException(string message) 
			: base(message)
		{
		}
	}
}