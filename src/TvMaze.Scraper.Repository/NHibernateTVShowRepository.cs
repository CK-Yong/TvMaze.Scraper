using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.Core.Domain;

namespace TvMaze.Scraper.Repository
{
	public class NHibernateTVShowRepository : IRepository<TvShow>
	{
		private readonly ISessionFactory _sessionFactory;

		public NHibernateTVShowRepository(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public async Task SaveAsync(TvShow entity, CancellationToken cancellationToken)
		{
			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				await session.MergeAsync(entity, cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}
		}

		public async Task<TvShow> GetAsync(int id, CancellationToken cancellationToken)
		{
			using (var session = _sessionFactory.OpenSession())
			{
				var fromDb = await session.QueryOver<TvShow>()
					.Where(show => show.Id == id)
					.Fetch(show => show.Cast).Eager
					.SingleOrDefaultAsync(cancellationToken);
				return fromDb;
			}
		}

		public Task<IList<TvShow>> GetMultipleAsync(int startIndex, int pageCount, CancellationToken cancellationToken)
		{
			using (var session = _sessionFactory.OpenSession())
			{
				return session.QueryOver<TvShow>()
					.Skip(startIndex - 1)
					.Take(pageCount)
					.ListAsync(cancellationToken);
			}
		}

		/// <summary>
		/// Gets ID of the last inserted TvShow asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns></returns>
		public async Task<int> GetLastIndexAsync(CancellationToken cancellationToken)
		{
			using (var session = _sessionFactory.OpenSession())
			{
				var fromDb = await session.QueryOver<TvShow>()
					.OrderBy(x => x.Id).Desc
					.Take(1).SingleOrDefaultAsync(cancellationToken);

				return fromDb.Id;
			}
		}
	}
}