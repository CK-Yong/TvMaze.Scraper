using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.Core.Domain;
using TvMaze.Scraper.TestUtilities;

namespace TvMaze.Scraper.Tests
{
	[TestFixture]
	public class TvMazeScraperTests : AsyncSpecification
	{
		private AutoMock _autoMock;
		private TvMazeScraper _scraper;
		private ScrapeResult<TvShow> _scrapeResult;
		private TvShow _tvShow;
		private Fixture _fixture;

		protected override void EstablishContext()
		{
			_autoMock = AutoMock.GetLoose();
			_scraper = _autoMock.Create<TvMazeScraper>();

			_fixture = new Fixture();
			_fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
			_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
		}

		public class When_scraping : TvMazeScraperTests
		{
			private int _id = 123;
			private ScrapeResult<TvShow> _result;

			public class When_successful : When_scraping
			{
				protected override void EstablishContext()
				{
					base.EstablishContext();
					_tvShow = _fixture.Build<TvShow>().With(x => x.Id, _id).Create();
					_scrapeResult = new ScrapeResult<TvShow>(_tvShow);

					_autoMock.Mock<ISource<TvShow>>()
						.Setup(m => m.GetByIdAsync(_id, It.IsAny<CancellationToken>()))
						.ReturnsAsync(_scrapeResult);
				}

				protected override async Task BecauseAsync()
				{
					_result = await _scraper.ScrapeAsync(_id, CancellationToken.None);
				}

				[Test]
				public void It_should_scrape_a_tvshow_from_the_source()
				{
					_autoMock.Mock<ISource<TvShow>>().Verify(m => m.GetByIdAsync(_id, It.IsAny<CancellationToken>()), Times.Once);
				}

				[Test]
				public void It_should_persist_the_tvshow_in_the_store()
				{
					_autoMock.Mock<IRepository<TvShow>>().Verify(m => m.SaveAsync(_tvShow, It.IsAny<CancellationToken>()), Times.Once);
				}

				[Test]
				public void It_should_return_the_scraped_result()
				{
					_result.Should().Be(_scrapeResult);
				}
			}

			public class When_something_goes_wrong : When_scraping
			{
				protected override void EstablishContext()
				{
					base.EstablishContext();
					_tvShow = _fixture.Build<TvShow>().With(x => x.Id, _id).Create();
					_scrapeResult = ScrapeResult<TvShow>.CreateError("Something went wrong.");

					_autoMock.Mock<ISource<TvShow>>()
						.Setup(m => m.GetByIdAsync(_id, It.IsAny<CancellationToken>()))
						.ReturnsAsync(_scrapeResult);
				}

				protected override async Task BecauseAsync()
				{
					_result = await _scraper.ScrapeAsync(_id, CancellationToken.None);
				}

				[Test]
				public void It_should_return_the_result()
				{
					_result.Should().Be(_scrapeResult);
				}

				[Test]
				public void The_result_should_indicate_non_success()
				{
					_result.IsSuccessful.Should().BeFalse();
				}
			}
		}
	}
}