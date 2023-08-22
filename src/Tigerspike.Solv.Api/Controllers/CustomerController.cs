using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.FeatureManagement.Mvc;
using Tigerspike.Solv.Api.Authentication.ApiKey;
using Tigerspike.Solv.Api.Authentication.Sdk;
using Tigerspike.Solv.Api.Models;
using Tigerspike.Solv.Application.Interfaces;
using Tigerspike.Solv.Application.Models.Customer;
using Tigerspike.Solv.Core.Bus;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Core.Enums;
using Tigerspike.Solv.Core.Extensions;
using Tigerspike.Solv.Core.Mediator;
using Tigerspike.Solv.Core.Mvc;
using Tigerspike.Solv.Core.Notifications;

namespace Tigerspike.Solv.Api.Controllers
{
	/// <summary>
	/// customers endpoint
	/// </summary>
	[ApiVersion("1.0")]
	[Route("v{api-version:apiVersion}/customers")]
	public class CustomerController : ApiController
	{
		private readonly ICustomerService _customerService;

		private readonly IBrandService _brandService;

		/// <summary>
		/// customer constructor
		/// </summary>
		public CustomerController(IDomainNotificationHandler notifications, IBrandService brandService,
			IMediatorHandler mediator, ICustomerService customerService) : base(notifications, mediator)
		{
			_customerService = customerService ??
				throw new ArgumentNullException(nameof(customerService));
			_brandService = brandService ??
				throw new ArgumentNullException(nameof(brandService));
		}

		/// <summary>
		/// Get customer tickets details
		/// </summary>
		/// <returns>
		/// 200 if successful with CustomerModel which include last 10 tickets of that customer
		/// </returns>
		[HttpGet("{customerEmail}")]
		[Authorize(AuthenticationSchemes = ApiKeyAuthentication.Scheme)]
		[ProducesResponseType(typeof(CustomerModel), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> GetTickets(string customerEmail)
		{
			if (await _brandService.CheckCustomerTicketsEndpointEnabled(User.GetBrandId()))
			{
				var result = await _customerService.GetTickets(customerEmail);
				if (result != null)
				{
					return Response(result);
				}
				else
				{
					return BadRequest("Invalid email address");
				}
			}

			return BadRequest();
		}
	}
}