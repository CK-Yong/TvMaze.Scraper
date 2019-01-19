using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoFixture;
using FluentAssertions;
using NHibernate;
using NUnit.Framework;
using TvMaze.Scraper.Core.Domain;
using TvMaze.Scraper.Repository.ClassMaps;
using TvMaze.Scraper.Repository.Tests.NHibernate;
using TvMaze.Scraper.TestUtilities;

namespace TvMaze.Scraper.Repository.Tests
{
	[TestFixture]
	internal class TvShowRepositoryIntegrationTests : AsyncSpecification
	{
		private AutoMock _mock;
		private ISessionFactory _sessionFactory;
		private NHibernateTVShowRepository _repository;
		private Fixture _fixture;
		private TvShow _lastSavedShow;
		private int _id = 1;

		protected override void EstablishContext()
		{
			_mock = AutoMock.GetLoose();
			_sessionFactory = _mock.UseInMemoryDatabase(GetType().FullName, configuration => configuration.FluentMappings.AddFromAssemblyOf<TvShowClassMap>());
			_repository = _mock.Create<NHibernateTVShowRepository>();
			_fixture = new Fixture();
			_fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
			_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
		}

		protected override void CleanUpContext()
		{
			_sessionFactory.Dispose();
		}

		private void GenerateAndSaveShows(int numberOfShows = 1)
		{
			var currentCount = 0;
			int lastCastId = 1;
			int lastTvShowId = _id;
			while (currentCount < numberOfShows)
			{
				_lastSavedShow = _fixture
					.Build<TvShow>()
					.With(x => x.Id, lastTvShowId)
					.Create();

				foreach (var cast in _lastSavedShow.Cast)
				{
					cast.Id = lastCastId;
					lastCastId++;
				}

				using (var session = _sessionFactory.OpenSession())
				using (var transaction = session.BeginTransaction())
				{
					session.Merge(_lastSavedShow);
					transaction.Commit();
				}

				lastTvShowId++;
				currentCount++;
			}
		}

		public class When_saving_the_tv_show : TvShowRepositoryIntegrationTests
		{
			private TvShow _input;

			protected override void EstablishContext()
			{
				base.EstablishContext();
				_input = _fixture
					.Build<TvShow>()
					.With(x => x.Id, _id)
					.Create();

				var i = 1; 
				foreach (var cast in _input.Cast)
				{
					cast.Id = i;
					i++;
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
					show.Should().BeEquivalentTo(_input, opts => opts.IgnoringCyclicReferences());
				}
			}
		}

		public class When_getting_the_tv_show : TvShowRepositoryIntegrationTests
		{
			private TvShow _result;

			protected override void EstablishContext()
			{
				base.EstablishContext();
				GenerateAndSaveShows();
			}

			protected override async Task BecauseAsync()
			{
				_result = await _repository.GetAsync(_id, CancellationToken.None);
			}

			[Test, Order(2)]
			public void It_should_get_the_show_by_id()
			{
				_result.Should().BeEquivalentTo(_lastSavedShow);
			}
		}

		public class When_getting_multiple_tv_shows : TvShowRepositoryIntegrationTests
		{
			private IEnumerable<TvShow> _result;

			protected override void EstablishContext()
			{
				base.EstablishContext();
				GenerateAndSaveShows(20);
			}

			protected override async Task BecauseAsync()
			{
				_result = await _repository.GetMultipleAsync(5, 10, CancellationToken.None);
			}

			[Test]
			public void It_should_get_the_specified_number_of_entities()
			{
				_result.Count().Should().Be(10, "because the specified page count was 10.");
			}

			[Test]
			public void It_should_start_at_the_entity_with_the_id_of_the_start_index()
			{
				_result.First().Id.Should().Be(5, "because the startindex is 5.");
			}
		}
	}
}