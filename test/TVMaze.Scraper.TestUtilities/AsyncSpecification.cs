using System.Threading.Tasks;

namespace TVMaze.Scraper.TestUtilities
{
    public abstract class AsyncSpecification : Specification
    {
        protected override void Because()
        {
            AsyncPump.Run(() => BecauseAsync());
        }

        protected override void EstablishContext()
        {
            AsyncPump.Run(() => EstablishContextAsync());
        }

        protected override void CleanUpContext()
        {
            AsyncPump.Run(() => CleanUpContextAsync());
        }

        protected virtual Task BecauseAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task EstablishContextAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task CleanUpContextAsync()
        {
            return Task.CompletedTask;
        }
    }
}
