using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TvMaze.Scraper.Core.Domain;

namespace TvMaze.Scraper.Core
{
	/// <summary>
	/// Repository that is to be used for saving and retrieving entities of type <see cref="T"/> from a persistence store.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRepository<T> 
	{
		/// <summary>
		/// Saves entity asynchronously.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task SaveAsync(T entity, CancellationToken cancellationToken);

		/// <summary>
		/// Gets the entity asynchronously.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<T> GetAsync(int id, CancellationToken cancellationToken);

		/// <summary>
		/// Gets multiple entities based on the specified indices asynchronously.
		/// </summary>
		/// <param name="elementsToSkip">The start index.</param>
		/// <param name="pageCount">The page count.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<TvShow>> GetMultipleAsync(int elementsToSkip, int pageCount, CancellationToken cancellationToken);

		/// <summary>
		/// Gets the total number of entitites of type <see cref="T"/> that are stored within the repository asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		Task<int> GetTotalItemsAsync(CancellationToken cancellationToken);
	}
}