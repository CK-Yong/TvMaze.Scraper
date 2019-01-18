using System;
using System.Collections.Generic;

namespace TvMaze.Scraper.Core
{
	public class ScrapeResult<TData>
	{
		private readonly TData _data;

		public readonly string ErrorMessage;

		public readonly bool IsSuccessful;

		public ScrapeResult(TData data)
		{
			if (EqualityComparer<TData>.Default.Equals(data, default(TData)))
			{
				throw new ArgumentNullException(nameof(data), "Cannot create a successful scrape result with null data.");
			}

			IsSuccessful = true;
			_data = data;
		}

		private ScrapeResult(string errorMessage)
		{
			ErrorMessage = errorMessage;
			IsSuccessful = false;
		}

		public TData Data
		{
			get
			{
				if (IsSuccessful)
				{
					return _data;
				}

				throw new ScrapeDataNotAccessibleException(ErrorMessage);
			}
		}

		/// <summary>
		/// Creates a faulted <see cref="ScrapeResult{T}"/> that throws a <see cref="ScrapeDataNotAccessibleException"/> with the given <param name="errorMessage">error message</param> when the data is accessed.
		/// </summary>
		/// <typeparam name="T">The type of the object that the result should hold.</typeparam>
		/// <param name="errorMessage">The error message.</param>
		/// <returns>An initialized <see cref="ScrapeResult{T}"/> that is faulted.</returns>
		public static ScrapeResult<TData> CreateError(string errorMessage)
		{
			return new ScrapeResult<TData>(errorMessage);
		}
	}
}