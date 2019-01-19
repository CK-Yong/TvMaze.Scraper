using FluentNHibernate.Mapping;
using TvMaze.Scraper.Core.Domain;

namespace TvMaze.Scraper.Repository.ClassMaps
{
	public class TvShowClassMap : ClassMap<TvShow>
	{
		public TvShowClassMap()
		{
			Id(x => x.Id).GeneratedBy.Assigned();
			Map(x => x.Name);
			HasMany(x => x.Cast)
				.KeyColumn("TvShowId")
				.Cascade.All();
		}
	}
}
