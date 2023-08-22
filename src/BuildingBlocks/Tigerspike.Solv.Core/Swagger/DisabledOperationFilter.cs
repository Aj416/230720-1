using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tigerspike.Solv.Core.Swagger
{
	/// <summary>
	/// Hides not defined operations
	/// </summary>
	public class DisabledOperationFilter : IDocumentFilter
	{
		/// <inheritdoc/>
		public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
		{
			foreach (var pathItem in swaggerDoc.Paths.Values)
			{
				foreach (var pathItemOperation in pathItem.Operations)
				{
					NullifyIfDisabledApi(pathItemOperation.Value);
				}
			}
		}

		private OpenApiOperation NullifyIfDisabledApi(OpenApiOperation operation)
		{
			if (operation == null)
			{
				return null;
			}

			return operation.Tags.Any(t => t.Name == "DisabledApi") ? null : operation;
		}
	}
}