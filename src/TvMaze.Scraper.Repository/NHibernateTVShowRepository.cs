using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using TvMaze.Scraper.Core;

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
				await session.SaveAsync(entity, cancellationToken);
				await transaction.CommitAsync(cancellationToken);
			}
		}

		public async Task<TvShow> GetAsync(int id, CancellationToken cancellationToken)
		{
			using (var session = _sessionFactory.OpenSession())
			{
				var fromDb = await session.QueryOver<TvShow>()
					.Where(show => show.Id == id)
					.Fetch(show => show.Cast).Default
					.SingleOrDefaultAsync(cancellationToken);
				return fromDb;
			}
		}
	}
}