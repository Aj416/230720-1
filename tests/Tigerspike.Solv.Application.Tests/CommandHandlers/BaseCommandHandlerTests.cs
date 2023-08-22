using AutoMoqCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Data;
using System.Threading;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Services;
using Tigerspike.Solv.Core.UnitOfWork;

namespace Tigerspike.Solv.Application.Tests.CommandsHandlers
{
	public abstract class BaseCommandHandlerTests<T>
	{

		protected readonly AutoMoqer Mocker = new AutoMoqer();
		protected Mock<IMediatorHandler> MediatorMock => Mocker.GetMock<IMediatorHandler>();

		private T _systemUnderTest;
		protected T SystemUnderTest 
		{
			get 
			{ 
				if (_systemUnderTest == null)
				{
					_systemUnderTest = Mocker.Resolve<T>();
				}
				
				return _systemUnderTest;
			}
		}


		protected BaseCommandHandlerTests()
		{
			Mocker.GetMock<IUnitOfWork>()
				.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(1);
				
			Mocker.GetMock<IUnitOfWork>()
				.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>()))
				.ReturnsAsync(new Mock<IDbContextTransaction>().Object);

			Mocker.SetInstance<ITimestampService>(new FixedTimestampService(DateTime.UtcNow));
		}
	}
}