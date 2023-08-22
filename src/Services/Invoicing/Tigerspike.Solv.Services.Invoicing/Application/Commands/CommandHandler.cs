using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;

namespace Tigerspike.Solv.Services.Invoicing.Application.CommandHandlers
{
	public class CommandHandler
	{
		public const string CommitErrorKey = "CommitError";
		protected readonly IMediatorHandler _mediator;
		private readonly IUnitOfWork _uow;
		private readonly IDomainNotificationHandler _notifications;

		public CommandHandler(IUnitOfWork uow, IMediatorHandler mediator, IDomainNotificationHandler notifications)
		{
			_uow = uow;
			_notifications = notifications;
			_mediator = mediator;
		}

		public Task<IDbContextTransaction> BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => _uow.BeginTransaction(isolationLevel);

		public async Task<bool> Commit()
		{
			if (_notifications.HasNotifications())
			{
				return false;
			}

			int commandResponse;

			try
			{
				commandResponse = await _uow.SaveChangesAsync();
			}
			catch (System.Exception ex)
			{
				await _mediator.RaiseEvent(new DomainNotification(CommitErrorKey, string.Join(" -> ", ex.WithInnerExceptions().Select(x => x.Message))));

				return false;
			}

			if (commandResponse > 0)
			{
				return true;
			}

			await _mediator.RaiseEvent(new DomainNotification(CommitErrorKey,
				"We had a problem during saving your data."));

			return false;
		}
	}
}
