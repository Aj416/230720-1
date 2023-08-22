using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Notifications;
using Tigerspike.Solv.Core.UnitOfWork;
using Tigerspike.Solv.Domain.Commands;
using Tigerspike.Solv.Domain.Commands.Brand;
using Tigerspike.Solv.Domain.Models.Induction;
using Tigerspike.Solv.Infra.Data.Interfaces;

namespace Tigerspike.Solv.Application.CommandHandlers.Brand
{
	public class BrandInductionCommandHandler : CommandHandler,
		IRequestHandler<CreateInductionSectionCommand, Unit>,
		IRequestHandler<CreateInductionSectionItemCommand, Unit>,
		IRequestHandler<UpdateInductionSectionCommand, Unit>,
		IRequestHandler<UpdateInductionSectionItemCommand, Unit>,
		IRequestHandler<DeleteInductionSectionCommand, Unit>,
		IRequestHandler<DeleteInductionSectionItemCommand, Unit>

	{
		private readonly IBrandRepository _brandRepository;
		private readonly IMapper _mapper;

		public BrandInductionCommandHandler(
			IBrandRepository brandRepository,
			IUnitOfWork uow,
			IMediatorHandler mediator,
			IMapper mapper,
			IDomainNotificationHandler notifications) : base(uow, mediator, notifications)
		{
			_brandRepository = brandRepository;
			_mapper = mapper;
		}



		public async Task<Unit> Handle(CreateInductionSectionCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				disableTracking: false,
				predicate: x => x.Id == request.BrandId,
				include: x => x.Include(inc => inc.InductionSections)
			);

			// create new section
			var section = new Section(request.SectionId, request.Name, true, request.BrandId, request.Order);
			brand.InductionSections.Add(section);
			_brandRepository.Update(brand);

			if (await Commit())
			{
				// raise event if you like
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to create new induction section for brand {request.BrandId} {brand.Name} / {request.Name}"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(CreateInductionSectionItemCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				disableTracking: false,
				predicate: x => x.Id == request.BrandId,
				include: x => x
					.Include(inc => inc.InductionSections)
					.ThenInclude(inc => inc.SectionItems)
			);

			// create new section
			var section = brand.InductionSections.FirstOrDefault(x => x.Id == request.SectionId);
			if (section != null)
			{
				var sectionItem = new SectionItem(request.SectionItemId, request.Name, request.Source, true, request.Order);
				section.SectionItems.Add(sectionItem);
				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to create new induction section for brand {request.BrandId} {brand.Name} / {request.Name}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to create new induction section for brand (parent section not exists) {request.BrandId} {brand.Name} / {request.Name}"));
			}


			return Unit.Value;
		}

		public async Task<Unit> Handle(UpdateInductionSectionCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				disableTracking: false,
				predicate: x => x.Id == request.BrandId,
				include: x => x.Include(inc => inc.InductionSections)
			);

			var toUpdate = brand.InductionSections.FirstOrDefault(x => x.Id == request.SectionId);
			if (toUpdate != null)
			{
				toUpdate.SetName(request.Name);
				toUpdate.SetOrder(request.Order);
				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to update induction section for brand {request.BrandId} {brand.Name} / {request.SectionId}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to update induction section for brand, because it does not exists {request.BrandId} {brand.Name} / {request.SectionId}"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(UpdateInductionSectionItemCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				disableTracking: false,
				predicate: x => x.Id == request.BrandId,
				include: x => x
					.Include(inc => inc.InductionSections)
					.ThenInclude(inc => inc.SectionItems)
			);

			var toUpdate = brand.InductionSections
				.SelectMany(x => x.SectionItems)
				.Where(x => x.Id == request.SectionItemId)
				.FirstOrDefault();

			if (toUpdate != null)
			{
				toUpdate.SetName(request.Name);
				toUpdate.SetOrder(request.Order);
				toUpdate.SetSource(request.Source);

				if (toUpdate.SectionId != request.SectionId)
				{
					var newSection = brand.InductionSections.FirstOrDefault(x => x.Id == request.SectionId);
					toUpdate.SetSection(newSection);
				}

				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to update induction section item for brand {request.BrandId} {brand.Name} / {request.SectionItemId}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to update induction section item because it does not exist {request.BrandId} {brand.Name} / {request.SectionItemId}"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(DeleteInductionSectionCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				disableTracking: false,
				predicate: x => x.Id == request.BrandId,
				include: x => x.Include(inc => inc.InductionSections)
			);

			var toRemove = brand.InductionSections.FirstOrDefault(x => x.Id == request.SectionId);
			if (toRemove != null)
			{
				brand.InductionSections.Remove(toRemove);
				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to delete induction section for brand {request.BrandId} {brand.Name} / {request.SectionId}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to delete induction section for brand, because it does not exists {request.BrandId} {brand.Name} / {request.SectionId}"));
			}

			return Unit.Value;
		}

		public async Task<Unit> Handle(DeleteInductionSectionItemCommand request, CancellationToken cancellationToken)
		{
			var brand = await _brandRepository.GetSingleOrDefaultAsync(
				disableTracking: false,
				predicate: x => x.Id == request.BrandId,
				include: x => x
					.Include(inc => inc.InductionSections)
					.ThenInclude(inc => inc.SectionItems)
			);

			var toRemove = brand.InductionSections
				.SelectMany(x => x.SectionItems)
				.Where(x => x.Id == request.SectionItemId)
				.FirstOrDefault();

			if (toRemove != null)
			{
				toRemove.Section.SectionItems.Remove(toRemove);
				_brandRepository.Update(brand);

				if (await Commit())
				{
					// raise event if you like
				}
				else
				{
					await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to delete induction section item for brand {request.BrandId} {brand.Name} / {request.SectionItemId}"));
				}
			}
			else
			{
				await _mediator.RaiseEvent(new DomainNotification(request.MessageType, $"Failed to delete induction section item because it does not exist {request.BrandId} {brand.Name} / {request.SectionItemId}"));
			}

			return Unit.Value;
		}
	}
}