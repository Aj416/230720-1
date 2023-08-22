using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tigerspike.Solv.Core.Extensions
{
	public static class EnumExtensions
	{
		/// <summary> Gets an attribute on an enum field value </summary>
		/// <returns>The attribute of type T that exists on the enum value</returns>
		public static T GetAttributeOfType<T>(this Enum enumVal) where T : Attribute
		{
			var type = enumVal.GetType();
			var memInfo = type.GetMember(enumVal.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
			return attributes.Length > 0 ? (T)attributes[0] : null;
		}

		/// <summary>
		/// Returns whether enum value is present in the list
		/// </summary>
		public static bool IsIn<T>(this T enumVal, params T[] list) where T : struct
		{
			return list.Contains(enumVal);
		}

		/// <summary>
		/// Returns whether enum value is not present in the list
		/// </summary>
		public static bool IsNotIn<T>(this T enumVal, params T[] list) where T : struct
		{
			return enumVal.IsIn(list) == false;
		}

		/// <summary>
		/// Returns display value of an enum
		/// </summary>
		public static string ToDisplay<T>(this T enumVal) where T : Enum
		{
			var displayAttribute = GetAttributeOfType<DisplayAttribute>(enumVal);
			return displayAttribute?.Name ?? enumVal.ToString();
		}

		/// <summary>
		/// Returns list of enums from the flat string
		/// </summary>
		public static IList<T> ToList<T>(this string enumList) where T : struct, Enum
		{
			return enumList?
				.Split(",")
				.Select(x => x.Trim())
				.Select(x => Enum.Parse<T>(x))
				.ToList();
		}

		public static IEnumerable<int> GetIntValues(Type enumVal)
		{
			var values = Enum.GetValues(enumVal);
			foreach (int value in values)
			{
				yield return value;
			}
		}
	}
}