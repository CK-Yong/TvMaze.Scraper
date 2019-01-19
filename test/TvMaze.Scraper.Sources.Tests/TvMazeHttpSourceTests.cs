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
using TvMaze.Scraper.Core.Domain;
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

		private void SetupOkResponse(string uri, StringContent content)
		{
			_handler.SetupRequest(HttpMethod.Get, new Uri(_baseUri, uri))
				.ReturnsResponse(HttpStatusCode.OK, content)
				.Verifiable();
		}

		public class When_getting_a_show_by_id : TvMazeHttpSourceTests
		{
			private int _id = 123;

			protected override void EstablishContext()
			{
				base.EstablishContext();
				SetupOkResponse($"shows/{_id}", TestStringContent.TvShow);
				SetupOkResponse($"shows/{_id}/cast", TestStringContent.Cast);
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

			private void SetupNotFoundResponse(string uri)
			{
				var resourceUri = new Uri(_baseUri, uri);
				_handler.SetupRequest(HttpMethod.Get, resourceUri)
					.ReturnsResponse(HttpStatusCode.NotFound, new StringContent(string.Empty),
						response =>
						{
							response.RequestMessage = new HttpRequestMessage();
							response.RequestMessage.RequestUri = resourceUri;
						})
					.Verifiable();
			}

			public class When_the_tv_show_request_fails : When_the_request_fails
			{
				protected override void EstablishContext()
				{
					base.EstablishContext();
					SetupNotFoundResponse($"shows/{_id}");
					SetupOkResponse($"shows/{_id}/cast", TestStringContent.Cast);
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
					_result.ErrorCode.Should().Be(ErrorCode.NotFound);
				}
			}

			public class When_the_cast_request_fails : When_the_request_fails
			{
				protected override void EstablishContext()
				{
					base.EstablishContext();
					SetupOkResponse($"shows/{_id}", TestStringContent.TvShow);
					SetupNotFoundResponse($"shows/{_id}/cast");
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
					_result.ErrorCode.Should().Be(ErrorCode.NotFound);
				}
			}
		}
	}
}