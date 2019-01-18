using System;
using System.Collections.Generic;

namespace TvMaze.Scraper.Core
{
	/// <summary>
	/// Marker class for scrape results.
	/// </summary>
	public abstract class ScrapeResult
	{
		/// <summary>
		/// Indicates whether the result was successful.
		/// </summary>
		public bool IsSuccessful;

		/// <summary>
		/// Used to communicate an error message.
		/// </summary>
		public string ErrorMessage;

		/// <summary>
		/// Initializes a new instance of the <see cref="ScrapeResult"/> class.
		/// </summary>
		protected ScrapeResult()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScrapeResult{TData}"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		protected ScrapeResult(string errorMessage)
		{
			ErrorMessage = errorMessage;
			IsSuccessful = false;
		}
	}

	/// <summary>
	/// Result that contains the retrieved data.
	/// </summary>
	/// <typeparam name="TData">The type of the data.</typeparam>
	public class ScrapeResult<TData> : ScrapeResult
	{
		private readonly TData _data;

		/// <summary>
		/// Initializes a new instance of the <see cref="ScrapeResult{TData}"/> class.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <exception cref="ArgumentNullException">data - Cannot create a successful scrape result with null data.</exception>
		public ScrapeResult(TData data)
		{
			if (EqualityComparer<TData>.Default.Equals(data, default(TData)))
			{
				throw new ArgumentNullException(nameof(data), "Cannot create a successful scrape result with null data.");
			}

			IsSuccessful = true;
			_data = data;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScrapeResult{TData}"/> class.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		public ScrapeResult(string errorMessage)
			: base(errorMessage)
		{
		}

		/// <summary>
		/// Gets the data associated with this <see cref="ScrapeResult{TData}"/>.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		/// <exception cref="ScrapeDataNotAccessibleException">If the result was unsuccessful.</exception>
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