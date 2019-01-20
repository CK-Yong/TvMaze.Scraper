using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Moq;
using NUnit.Framework;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.Core.Domain;
using TvMaze.Scraper.TestUtilities;
using TvMaze.Scraper.WebHost.Controllers;
using TvMaze.Scraper.WebHost.Controllers.Paging;

namespace Tests
{
	[TestFixture]
	public class TvShowControllerTests : AsyncSpecification
	{
		private AutoMock _autoMock;
		private TvShowController _controller;
		private IActionResult _result;

		public class When_getting_a_collection_of_shows : TvShowControllerTests
		{
			private IList<TvShow> _repositoryResult;
			private int _page = 1;
			private int _pageCount = 123;
			private int _totalNumberItems;
			private Uri _originalRequestUri;

			protected override void EstablishContext()
			{
				_autoMock = AutoMock.GetLoose();
				_controller = _autoMock.Create<TvShowController>();

				var fixture = new Fixture();
				_repositoryResult = fixture.Create<IList<TvShow>>();

				_autoMock.Mock<IRepository<TvShow>>()
					.Setup(m => m.GetMultipleAsync(0, _pageCount, It.IsAny<CancellationToken>()))
					.ReturnsAsync(_repositoryResult)
					.Verifiable();

				_totalNumberItems = 50;
				_autoMock.Mock<IRepository<TvShow>>()
					.Setup(m => m.GetTotalItemsAsync(It.IsAny<CancellationToken>()))
					.ReturnsAsync(_totalNumberItems)
					.Verifiable();

				_originalRequestUri = new Uri($"http://0.0.0.0/api/shows?page={_page}&pagesize={_pageCount}");
				SetupOriginalRequest(_controller, _originalRequestUri, HttpMethod.Get);
			}

			private void SetupOriginalRequest(ControllerBase controller, Uri requestUri, HttpMethod method)
			{
				controller.ControllerContext = new ControllerContext();
				controller.ControllerContext.HttpContext = new DefaultHttpContext();
				var originalRequest = controller.ControllerContext.HttpContext.Request;
				originalRequest.Host = new HostString(_originalRequestUri.Host);
				originalRequest.Path = requestUri.AbsolutePath;
				originalRequest.Scheme = requestUri.Scheme;
				originalRequest.QueryString = new QueryString(requestUri.Query);
				originalRequest.Method = method.ToString().ToUpper();
			}

			protected override async Task BecauseAsync()
			{
				_result = await _controller.GetListAsync(CancellationToken.None, _page, _pageCount);
			}

			[Test]
			public void It_should_get_the_list_and_total_number_of_items_from_the_repository()
			{
				_autoMock.Mock<IRepository<TvShow>>().Verify();
			}

			[Test]
			public void It_should_return_an_ok_result()
			{
				_result.Should().BeOfType<OkObjectResult>();
			}

			[Test]
			public void It_should_contain_the_content_from_the_repository()
			{
				var paginatedResult = ((PaginatedResult<TvShow>) ((OkObjectResult) _result).Value).Data;
				paginatedResult.Should().BeEquivalentTo(_repositoryResult);
			}

			[Test]
			public void It_should_contain_paging_metadata()
			{
				var paginatedResult = (PaginatedResult<TvShow>) ((OkObjectResult) _result).Value;
				paginatedResult.Paging.Should().BeEquivalentTo(new PagingMetadata(_page, _pageCount, _totalNumberItems));
			}

			[Test]
			public void It_should_contain_the_links_to_the_next_pages()
			{
				var paginatedResult = (PaginatedResult<TvShow>) ((OkObjectResult) _result).Value;
				paginatedResult.Links.Should().BeEquivalentTo(new PageLinks(
					_originalRequestUri,
					new Uri($"http://0.0.0.0/api/shows?page={_page + 1}&pagesize={_pageCount}"),
					"GET"));
			}

			[Test]
			public void It_should_have_sorted_the_cast_members_by_birthday_descending()
			{
				var paginatedResult = (PaginatedResult<TvShow>) ((OkObjectResult) _result).Value;
				foreach (var show in paginatedResult.Data)
				{
					show.Cast.Should().BeInDescendingOrder(cast => cast.Birthday);
				}
			}
		}
	}
}