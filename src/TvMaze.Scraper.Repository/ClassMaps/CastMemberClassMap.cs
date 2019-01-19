using FluentNHibernate.Mapping;
using TvMaze.Scraper.Core.Domain;

namespace TvMaze.Scraper.Repository.ClassMaps
{
	public class CastMemberClassMap : ClassMap<CastMember>
	{
		public CastMemberClassMap()
		{
			Table("CastMembers");
			Id(x => x.Id).GeneratedBy.Assigned();
			Map(x => x.Name).Not.Nullable();
			Map(x => x.Birthday).Nullable();
		}
	}
}