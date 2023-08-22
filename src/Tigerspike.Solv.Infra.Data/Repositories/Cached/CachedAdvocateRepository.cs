using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Redis;
using Tigerspike.Solv.Core.Constants;
using Tigerspike.Solv.Infra.Data.Interfaces;
using Tigerspike.Solv.Infra.Data.Interfaces.Cached;
using Tigerspike.Solv.Infra.Data.Models;
using static Tigerspike.Solv.Core.Constants.CacheKeys;

namespace Tigerspike.Solv.Infra.Data.Repositories.Cached
{
	public class CachedAdvocateRepository : ICachedAdvocateRepository
	{
		private readonly IAdvocateRepository _advocateRepository;
		private readonly IRedisClientsManager _redisClientsManager;

		public CachedAdvocateRepository(
			IAdvocateRepository advocateRepository,
			IRedisClientsManager redisClientsManager
			)
		{
			_advocateRepository = advocateRepository ?? throw new ArgumentNullException(nameof(advocateRepository));
			_redisClientsManager = redisClientsManager ?? throw new ArgumentNullException(nameof(redisClientsManager));
		}

		/// <inheritdoc />
		public async Task<List<UserEmailModel>> GetSuperSolvers(Guid brandId)
		{
			var cacheKey = GetBrandSuperSolvers(brandId);
			using var client = _redisClientsManager.GetClient();
			var typedClient = client.As<List<UserEmailModel>>();
			if (!client.ContainsKey(cacheKey))
			{
				var list = await GetSuperSolversFromDb(brandId);
				typedClient.SetValue(cacheKey, list, TimeSpan.FromHours(1));
			}

			return typedClient.GetValue(cacheKey);
		}

		/// <inheritdoc />
		public Task<List<Guid>> GetOnlineAdvocates(Guid brandId)
		{
			using var client = _redisClientsManager.GetClient();
			return Task.FromResult(client.Sets[GetBrandOnlineAdvocatesKey(brandId)]
				.Select(id => new Guid(id)).ToList());
		}

		/// <inheritdoc />
		public async Task<bool> GetInternalAgentInfo(Guid advocateId)
		{
			using (var redisClient = _redisClientsManager.GetClient())
			{
				var cacheKey = CacheKeys.AdvocateInternalAgentInfoKey(advocateId);
				if (redisClient.ContainsKey(cacheKey) == false)
				{
					var cacheValue = await _advocateRepository.GetFirstOrDefaultAsync(x => x.InternalAgent, x => x.Id == advocateId);
					redisClient.Set(cacheKey, cacheValue, TimeSpan.FromDays(1));
				}

				return redisClient.Get<bool>(cacheKey);
			}
		}

		private Task<List<UserEmailModel>> GetSuperSolversFromDb(Guid brandId)
		{
			return _advocateRepository.Queryable()
				.Include(i => i.User)
				.Include(i => i.Brands)
				.Where(a => a.Super && a.Brands.Any(b => b.BrandId == brandId))
				.Select(a => new UserEmailModel(a.Id, a.User.FirstName, a.User.Email))
				.ToListAsync();
		}
	}
}