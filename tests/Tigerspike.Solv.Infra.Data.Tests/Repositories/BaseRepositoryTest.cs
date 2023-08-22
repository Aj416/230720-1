using System;
using AutoMoqCore;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Infra.Data.Context;

namespace Tigerspike.Solv.Infra.Data.Tests.Repositories
{
	public abstract class BaseRepositoryTest<T> : IDisposable
	{
		protected readonly AutoMoqer Mocker = new AutoMoqer();
		protected readonly SolvDbContext DbContext;

		private T _systemUnderTest;
		protected T SystemUnderTest
		{
			get
			{
				if (_systemUnderTest == null)
				{
					_systemUnderTest = Mocker.Create<T>();
				}

				return _systemUnderTest;
			}
		}

		public BaseRepositoryTest()
		{
			var dbName = Guid.NewGuid().ToString(); // use new db per test
			var builder = new DbContextOptionsBuilder<SolvDbContext>()
				.UseInMemoryDatabase(dbName);
				// .UseInMemoryDatabase("xunit-db");

			DbContext = new SolvDbContext(builder.Options);
			DbContext.Database.EnsureDeleted();
			DbContext.Database.EnsureCreated();

			Mocker.SetInstance(DbContext);
		}

		public void Dispose()
		{
			DbContext.Dispose();
		}
	}
}