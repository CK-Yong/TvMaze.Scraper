using FluentNHibernate.Mapping;
using TvMaze.Scraper.Core;

namespace TvMaze.Scraper.Repository.ClassMaps
{
	public class CastMemberClassMap : ClassMap<CastMember>
	{
		public CastMemberClassMap()
		{
			Id(x => x.Id).GeneratedBy.Identity();
			Map(x => x.Name);
			Map(x => x.Birthday);
			References(x => x.TvShow);
		}
	}
}