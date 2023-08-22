using System;
using DotLiquid;
using DotLiquid.NamingConventions;

namespace Tigerspike.Solv.Core.Email
{

	public static class LiquidStaticInitializer
	{


		static LiquidStaticInitializer()
		{
			Template.NamingConvention = new RubyNamingConvention();
			Template.RegisterFilter(typeof(SolvLiquidFilters));
		}

		public static void RegisterType<T>()
		{
			RegisterType(typeof(T));
		}
		public static void RegisterType(Type t)
		{
			if (t.IsEnum)
			{
				Template.RegisterSafeType(t, x => x.ToString());
			}
			else
			{
				Template.RegisterSafeType(t, x => Hash.FromAnonymousObject(x));
			}
		}
		public static void RegisterAssemblyWith<T>()
		{
			var objectType = typeof(T);
			foreach (var t in objectType.Assembly.GetTypes())
			{
				RegisterType(t);
			}
		}


	}
}