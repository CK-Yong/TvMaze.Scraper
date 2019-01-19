using FluentNHibernate.Mapping;
using TvMaze.Scraper.Core.Domain;

namespace TvMaze.Scraper.Repository.ClassMaps
{
	public class CastMemberClassMap : ClassMap<CastMember>
	{
		public CastMemberClassMap()
		{
			Id(x => x.Id).GeneratedBy.Assigned();
			Map(x => x.Name);
			Map(x => x.Birthday).Nullable();
			References(x => x.TvShow);
		}
	}
}