using System.Collections.Generic;
using DotLiquid;

namespace Tigerspike.Solv.Core.Email
{
	public class TemplateService : ITemplateService
	{
		public TemplateService()
		{
			LiquidStaticInitializer.RegisterAssemblyWith<TemplateService>();
		}

		/// <inheritdoc />
		public string Render(string template, params object[] inputs)
		{
			var model = GetHashModel(inputs);
			var t = Template.Parse(template);
			var options = new TemplateOptions { LocalVariables = model };
			return t.Render(options);
		}

		private Hash GetHashModel(params object[] inputs)
		{
			var model = new Hash();

			foreach (var input in inputs)
			{
				var hash = GetHashModel(input);
				foreach (var item in hash)
				{
					model.Add(item);
				}
			}

			return model;
		}

		private Hash GetHashModel(object input)
		{
			var dictionary = input as IDictionary<string, object>;
			if (dictionary != null)
			{
				return Hash.FromDictionary(dictionary);
			}
			else
			{
				if (input != null)
				{
					return Hash.FromAnonymousObject(input);
				}
				else
				{
					return Hash.FromAnonymousObject(new object());
				}

			}
		}
	}
}