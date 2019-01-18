using TvMaze.Scraper.Sources.Models;

namespace TvMaze.Scraper.Sources.Extensions
{
	internal static class CastMemberExtensions
	{
		/// <summary>
		/// Converts the <see cref="MapToCore"/> to a <see cref="TvMaze.Scraper.Core.CastMember"/>.
		/// </summary>
		/// <param name="castMember">The cast member.</param>
		/// <returns></returns>
		public static Core.CastMember MapToCore(this CastMember castMember)
		{
			return new Core.CastMember
			{
				Id = castMember.Person.Id,
				Birthday = castMember.Person.Birthday,
				Name = castMember.Person.Name
			};
		}
	}
}