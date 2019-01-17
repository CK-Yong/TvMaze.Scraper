using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoFixture;
using Moq;
using NUnit.Framework;
using TvMaze.Scraper;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.Core.Classes;
using TVMaze.Scraper.TestUtilities;

namespace Tests
{
	[TestFixture]
	public class TvMazeScraperTests : AsyncSpecification
	{
		private AutoMock _autoMock;
		private TvMazeScraper _scraper;
		private int _id = 123;
		private TvShow _scrapeResult;

		protected override void EstablishContext()
		{
			_autoMock = AutoMock.GetLoose();
			_scraper = _autoMock.Create<TvMazeScraper>();

			var fixture = new Fixture();
			fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());

			_scrapeResult = fixture.Build<TvShow>().With(x => x.Id, _id).Create();

			_autoMock.Mock<ISource<TvShow>>()
				.Setup(m => m.GetByIdAsync(_id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(_scrapeResult);
		}

		protected override Task BecauseAsync()
		{
			return _scraper.ScrapeAsync(_id, CancellationToken.None);
		}

		[Test]
		public void It_should_scrape_a_tvshow_from_the_source()
		{
			_autoMock.Mock<ISource<TvShow>>().Verify(m => m.GetByIdAsync(_id, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Test]
		public void It_should_persist_the_tvshow_in_the_store()
		{
			_autoMock.Mock<IRepository<TvShow>>().Verify(m => m.SaveAsync(_scrapeResult, It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}