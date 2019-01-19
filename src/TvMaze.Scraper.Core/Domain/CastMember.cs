using System;

namespace TvMaze.Scraper.Core.Domain
{
	public class CastMember 
	{
		public virtual int Id { get; set; }
		public virtual DateTime? Birthday { get; set; }
		public virtual string Name { get; set; }
	}
}