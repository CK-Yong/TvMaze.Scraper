using System.Threading;
using System.Threading.Tasks;

namespace TvMaze.Scraper.Core
{
	/// <summary>
	/// Source that provides entities.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ISource<T>
	{
		/// <summary>
		/// Gets the entity of type <see cref="T"/> by identifier asynchronously.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<T> GetByIdAsync(int id, CancellationToken cancellationToken);
	}
}