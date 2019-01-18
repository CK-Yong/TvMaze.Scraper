using System;
using FluentAssertions;
using NUnit.Framework;
using TvMaze.Scraper.TestUtilities;


namespace TvMaze.Scraper.Core.Tests
{
	[TestFixture]
	public class ScrapeResultTests : Specification
	{
		private ScrapeResult<object> _scraped;
		private object _data;

		public class When_accessing_valid_data : ScrapeResultTests
		{
			private object _actual;

			protected override void EstablishContext()
			{
				_data = new object();
				_scraped = new ScrapeResult<object>(_data);
			}

			protected override void Because()
			{
				_actual = _scraped.Data;
			}

			[Test]
			public void It_should_get_the_data()
			{
				_actual.Should().Be(_data);
			}
		}

		public class When_the_data_is_null : ScrapeResultTests
		{
			private Action _action;

			protected override void Because()
			{
				_action = () => new ScrapeResult<object>(null);
			}

			[Test]
			public void It_should_get_the_data()
			{
				var ex = _action.Should().Throw<ArgumentNullException>().Which;
				ex.Message.Should().Contain("Cannot create a successful scrape result with null data.");
				ex.ParamName.Should().Be("data");
			}
		}

		public class When_creating_a_fault : ScrapeResultTests
		{
			protected override void Because()
			{
				_scraped = ScrapeResult<object>.CreateError("An error occurred.");
			}

			[Test]
			public void The_result_should_be_unsuccessful()
			{
				_scraped.IsSuccessful.Should().BeFalse();
			}
		}

		public class When_the_result_indicates_non_success : ScrapeResultTests
		{
			private Action _action;

			protected override void EstablishContext()
			{
				base.EstablishContext();
				_scraped = ScrapeResult<object>.CreateError("An error occurred.");
			}

			protected override void Because()
			{
				_action = () =>
				{
					var data = _scraped.Data;
				};
			}

			[Test]
			public void The_result_should_be_unsuccessful()
			{
				_action.Should().Throw<ScrapeDataNotAccessibleException>()
					.Which.Message.Should().Be("An error occurred.");
			}
		}
	}
}