using NUnit.Framework;

namespace TvMaze.Scraper.TestUtilities
{
    public abstract class Specification
    {
	    [OneTimeSetUp]
	    public void SetUp()
	    {
		    EstablishContext();
		    Because();
	    }

		[OneTimeTearDown]
		public void TearDown()
        {
            CleanUpContext();
        }

	    protected virtual void EstablishContext()
	    {
	    }

	    protected virtual void Because()
        {
        }

	    protected virtual void CleanUpContext()
        {
        }
    }
}
