using Autofac;

namespace TvMaze.Scraper
{
	/// <summary>
	/// Registers the <see cref="TvMazeScraper"/>.
	/// </summary>
	/// <seealso cref="Autofac.Module" />
	public class TvMazeScraperModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<TvMazeScraper>()
				.AsSelf();
		}
	}
}
