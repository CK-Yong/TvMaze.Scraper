using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using MaxKagamine.Moq.HttpClient;
using Moq;
using NUnit.Framework;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.TestUtilities;

namespace TvMaze.Scraper.Sources.Tests
{
	[TestFixture]
	internal class TvMazeHttpSourceTests : AsyncSpecification
	{
		private AutoMock _autoMock;
		private ISource<TvShow> _source;
		private ScrapeResult<TvShow> _result;
		private Mock<HttpMessageHandler> _handler;
		private Uri _baseUri = new Uri("http://0.0.0.0");

		protected override void EstablishContext()
		{
			_autoMock = AutoMock.GetLoose();
			_handler = new Mock<HttpMessageHandler>();
			_autoMock.Provide(new HttpClient(_handler.Object) {BaseAddress = _baseUri});
			_source = _autoMock.Create<TvMazeHttpSource>();
		}

		public class When_getting_a_show_by_id : TvMazeHttpSourceTests
		{
			private int _id = 123;

			protected override void EstablishContext()
			{
				base.EstablishContext();
				_handler.SetupRequest(HttpMethod.Get, new Uri(_baseUri, $"shows/{_id}"))
					.ReturnsResponse(HttpStatusCode.OK, TestStringContent.TvShow)
					.Verifiable();

				_handler.SetupRequest(HttpMethod.Get, new Uri(_baseUri, $"shows/{_id}/cast"))
					.ReturnsResponse(HttpStatusCode.OK, TestStringContent.Cast)
					.Verifiable();
			}

			protected override async Task BecauseAsync()
			{
				_result = await _source.GetByIdAsync(_id, CancellationToken.None);
			}

			[Test]
			public void It_should_make_both_calls_to_the_tvmaze_api()
			{
				_handler.VerifyAll();
			}

			[Test]
			public void It_should_have_deserialized_the_returned_content()
			{
				_result.Data.Id.Should().Be(1, "Because the test content is of ID 1.");
				_result.Data.Cast.Count().Should().Be(15, "Because there are 15 cast members in the show.");
			}
		}
	}
}