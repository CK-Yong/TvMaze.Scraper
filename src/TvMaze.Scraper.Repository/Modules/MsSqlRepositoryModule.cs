using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Driver;
using TvMaze.Scraper.Core;
using TvMaze.Scraper.Core.Domain;

namespace TvMaze.Scraper.Repository.Modules
{
	/// <summary>
	/// Module for registering the data access layer. Uses local SQLExpress by default.
	/// </summary>
	/// <seealso cref="Autofac.Module" />
	public class MsSqlRepositoryModule : Module
	{
		private string _connectionString = "Server=localhost\\SQLEXPRESS;Database=TvMazeScraper;Trusted_Connection=True;";

		/// <summary>
		/// Signals the module that the specified connectionstring should be used to connect to the database.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public MsSqlRepositoryModule ConnectionString(string connectionString)
		{
			_connectionString = connectionString;
			return this;
		}

		protected override void Load(ContainerBuilder builder)
		{
			var sessionFactory = Fluently.Configure()
				.Database(MsSqlConfiguration.MsSql2012
					.ConnectionString(_connectionString)
					.Driver<SqlClientDriver>())
				.Mappings(m => m.FluentMappings.AddFromAssembly(ThisAssembly))
				.BuildSessionFactory();

			builder.RegisterInstance(sessionFactory)
				.As<ISessionFactory>();

			builder.RegisterType<NHibernateTVShowRepository>()
				.As<IRepository<TvShow>>();
		}
	}
}