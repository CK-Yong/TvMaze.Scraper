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

			[Test]
			public void It_should_return_a_valid_result()
			{
				_result.IsSuccessful.Should().BeTrue();
			}
		}

		public class When_the_request_fails : TvMazeHttpSourceTests
		{
			private int _id = 123;

			protected override void EstablishContext()
			{
				base.EstablishContext();
				var showUri = new Uri(_baseUri, $"shows/{_id}");
				SetupNotFoundResponse(showUri);

				var castUri = new Uri(_baseUri, $"shows/{_id}/cast");
				SetupNotFoundResponse(castUri);
			}

			private void SetupNotFoundResponse(Uri castUri)
			{
				_handler.SetupRequest(HttpMethod.Get, castUri)
					.ReturnsResponse(HttpStatusCode.NotFound, new StringContent(string.Empty),
						response =>
						{
							response.RequestMessage = new HttpRequestMessage();
							response.RequestMessage.RequestUri = castUri;
						})
					.Verifiable();
			}

			protected override async Task BecauseAsync()
			{
				_result = await _source.GetByIdAsync(_id, CancellationToken.None);
			}

			[Test]
			public void It_should_return_an_unsuccessful_result()
			{
				_result.IsSuccessful.Should().BeFalse();
			}

			[Test]
			public void It_should_contain_a_relevant_error_message()
			{
				_result.ErrorMessage.Should().Contain("the remote API returned 404 - NotFound");
			}

			[Test]
			public void It_should_indicate_a_not_found_result()
			{
				_result.ErrorCode.Should().Be((int) HttpStatusCode.NotFound, "because the statuscode is 404.");
			}
		}
	}
}