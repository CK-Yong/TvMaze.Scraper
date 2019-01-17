using System;

namespace TvMaze.Scraper.Core.Classes
{
	public class CastMember 
	{
		public virtual TvShow TvShow { get; set; }
		public virtual int Id { get; set; }
		public virtual DateTime Birthday { get; set; }
		public virtual string Name { get; set; }
	}
}