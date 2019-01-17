using System.Threading;
using System.Threading.Tasks;

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
	}
}