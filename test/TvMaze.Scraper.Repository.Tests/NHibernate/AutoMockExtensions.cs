using System;
using Autofac.Extras.Moq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Moq;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace TvMaze.Scraper.Repository.Tests.NHibernate
{
	internal static class AutoMockExtensions
	{
		private const string DefaultInstanceName = "sharedmemdb";

		/// <summary>
		/// Configures <see cref="AutoMock"/> so that the <see cref="ISessionFactory"/> returns singleton <see cref="ISession"/> from the mock container, and when a transaction is requested from the session, it returns a singleton <see cref="ITransaction"/>.
		/// Should only be used if NHibernate in your active test(s) is irrelevant.
		/// </summary>
		public static ISessionFactory UseMockedDatabase(this AutoMock autoMock)
		{
			Mock<ISession> sessionMock = autoMock.Mock<ISession>();
			sessionMock
				.Setup(m => m.BeginTransaction())
				.Returns(autoMock.Mock<ITransaction>().Object);

			// Create new instance explicitly, instead of via AutoMock.
			Mock<ISessionFactory> sessionFactoryMock = new Mock<ISessionFactory>();
			sessionFactoryMock
				.Setup(m => m.OpenSession())
				.Returns(sessionMock.Object);

			autoMock.Provide(sessionFactoryMock.Object);
			return sessionFactoryMock.Object;
		}

		/// <summary>
		/// Configures <see cref="AutoMock"/> with an in memory database (SQLite) and NHibernate, and runs the migrations on it as provided by the <paramref name="mappingConfiguration"/>.
		/// </summary>
		/// <param name="autoMock">The <see cref="AutoMock"/> instance to provide with NHibernate session factory.</param>
		/// <param name="instanceName">The in memory instance name. Use the same instance name if you need to share in process with other test runs.</param>
		/// <param name="mappingConfiguration">The migration mappings to execute on each new memory instance of the database.</param>
		public static ISessionFactory UseInMemoryDatabase(this AutoMock autoMock, string instanceName, Action<MappingConfiguration> mappingConfiguration)
		{
			ISessionFactory sessionFactory = Fluently.Configure()
				.Database(
					SQLiteConfiguration.Standard
						.ConnectionString(c => c.Is($"Data Source={instanceName ?? DefaultInstanceName};Mode=Memory;Cache=Shared;Version=3;New=True;"))
						.ShowSql()
				)
				.ExposeConfiguration(c =>
				{
					new SchemaExport(c).Execute(true, true, false);
					c.SetProperty("connection.release_mode", "on_close");
				})
				.Mappings(mappingConfiguration)
				.BuildSessionFactory();

			autoMock.Provide(sessionFactory);
			return sessionFactory;
		}
	}
}
