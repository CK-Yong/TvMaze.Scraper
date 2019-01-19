using System.Collections.Generic;

namespace TvMaze.Scraper.Core.Domain
{
	public class TvShow 
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IEnumerable<CastMember> Cast { get; set; }
	}
}