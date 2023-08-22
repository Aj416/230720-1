using System.Linq;
using System.Reflection;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tigerspike.Solv.Core.Swagger
{
	/// <summary>
	/// Enabled/disables endpoints based on feature flag configuration
	/// </summary>
	public class FeatureToggleFilter : IOperationFilter
	{
		private readonly IFeatureManager _featureManager;

		/// <inheritdoc/>
		public FeatureToggleFilter(IFeatureManager featureManager) => _featureManager = featureManager;

		/// <inheritdoc/>
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var featureGateAttributes = context.MethodInfo.GetCustomAttributes<FeatureGateAttribute>(true).ToList();

			if(featureGateAttributes.Any(f => !f.Features.Any(f => _featureManager.IsEnabledAsync(f).Result)))
			{
				operation.Tags.Add(new OpenApiTag() { Name = "DisabledApi"});
			}
		}
	}
}