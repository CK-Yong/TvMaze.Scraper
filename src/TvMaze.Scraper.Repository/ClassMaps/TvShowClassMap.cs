using FluentNHibernate.Mapping;
using TvMaze.Scraper.Core.Domain;

namespace TvMaze.Scraper.Repository.ClassMaps
{
	public class TvShowClassMap : ClassMap<TvShow>
	{
		public TvShowClassMap()
		{
			Table("TvShows");
			Id(x => x.Id).GeneratedBy.Assigned();
			Map(x => x.Name);
			HasMany(x => x.Cast)
				.Not.LazyLoad()
				.KeyColumn("TvShow_Id")
				.Cascade.All();
		}
	}
}
