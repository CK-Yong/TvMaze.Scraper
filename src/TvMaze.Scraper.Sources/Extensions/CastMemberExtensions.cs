using TvMaze.Scraper.Sources.Models;

namespace TvMaze.Scraper.Sources.Extensions
{
	internal static class CastMemberExtensions
	{
		/// <summary>
		/// Converts the <see cref="MapToCore"/> to a <see cref="Core.Domain.CastMember"/>.
		/// </summary>
		/// <param name="castMember">The cast member.</param>
		/// <returns></returns>
		public static Core.Domain.CastMember MapToCore(this CastMember castMember)
		{
			return new Core.Domain.CastMember
			{
				Id = castMember.Person.Id,
				Birthday = castMember.Person.Birthday,
				Name = castMember.Person.Name
			};
		}
	}
}