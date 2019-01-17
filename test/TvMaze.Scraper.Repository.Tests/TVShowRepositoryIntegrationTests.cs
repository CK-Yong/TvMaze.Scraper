using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoFixture;
using FluentAssertions;
using NHibernate;
using NUnit.Framework;
using TvMaze.Scraper.Core.Classes;
using TvMaze.Scraper.Repository.ClassMaps;
using TvMaze.Scraper.Repository.Tests.NHibernate;
using TVMaze.Scraper.TestUtilities;

namespace TvMaze.Scraper.Repository.Tests
{
	[TestFixture]
	[SingleThreaded]
	internal class TvShowRepositoryIntegrationTests : AsyncSpecification
	{
		private AutoMock _mock;
		private ISessionFactory _sessionFactory;
		private NHibernateTVShowRepository _repository;
		private Fixture _fixture;
		private ISession _session; // Needed to maintain a connection to in-memory db.

		protected override void EstablishContext()
		{
			_mock = AutoMock.GetLoose();
			_sessionFactory = _mock.UseInMemoryDatabase(GetType().FullName, configuration => configuration.FluentMappings.AddFromAssemblyOf<TvShowClassMap>());
			_session = _sessionFactory.OpenSession();
			_repository = _mock.Create<NHibernateTVShowRepository>();
			_fixture = new Fixture();
			_fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
			_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
		}

		protected override void CleanUpContext()
		{
			_session.Dispose();
			_sessionFactory.Dispose();
		}

		public class When_saving_the_tv_show : TvShowRepositoryIntegrationTests
		{
			private TvShow _input;
			private int _id = 1;

			protected override void EstablishContext()
			{
				base.EstablishContext();
				_input = _fixture
					.Build<TvShow>()
					.With(x => x.Id, default(int))
					.Create();

				foreach (var cast in _input.Cast)
				{
					cast.Id = default(int);
				}
			}

			protected override Task BecauseAsync()
			{
				return _repository.SaveAsync(_input, CancellationToken.None);
			}

			[Test, Order(1)]
			public void It_should_persist_the_show()
			{
				using (var session = _sessionFactory.OpenSession())
				{
					var show = session.QueryOver<TvShow>().SingleOrDefault();
					show.Should().BeEquivalentTo(_input);
				}
			}
		}

		public class When_getting_the_tv_show : TvShowRepositoryIntegrationTests
		{
			private TvShow _expected;
			private int _id = 1;
			private TvShow _result;

			protected override void EstablishContext()
			{
				base.EstablishContext();
				_expected = _fixture
					.Build<TvShow>()
					.With(x => x.Id, default(int))
					.Create();

				foreach (var cast in _expected.Cast)
				{
					cast.Id = default(int);
				}

				using(var session = _sessionFactory.OpenSession())
				using(var transaction = session.BeginTransaction())
				{
					session.SaveOrUpdate(_expected);
					transaction.Commit();
				}
			}

			protected override async Task BecauseAsync()
			{
				_result = await _repository.GetAsync(_id, CancellationToken.None);
			}

			[Test, Order(2)]
			public void It_should_get_the_show_by_id()
			{
				_session.Lock(_result, LockMode.None); // Attach session to object.
				_result.Should().BeEquivalentTo(_expected);
			}
		}
	}
}