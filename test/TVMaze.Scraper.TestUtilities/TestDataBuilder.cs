namespace TvMaze.Scraper.TestUtilities
{
    public abstract class TestDataBuilder<T>
    {
        public T Build()
        {
            OnPreBuild();
            T subject = OnBuild();
            OnPostBuild(subject);

            return subject;
        }

        protected virtual void OnPreBuild()
        {
        }

        protected abstract T OnBuild();

        protected virtual void OnPostBuild(T subject)
        {
        }
    }
}
