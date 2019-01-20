using System.Diagnostics.CodeAnalysis;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace TvMaze.Scraper.WebHost
{
	[ExcludeFromCodeCoverage]
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
				.ConfigureServices(services => services.AddAutofac())
				.UseStartup<Startup>();
	}
}
