using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Ganss.XSS;
using Newtonsoft.Json;
using ServiceStackStringExtensions = ServiceStack.StringExtensions;

namespace Tigerspike.Solv.Core.Extensions
{
	public static class StringExtensions
	{
		static Regex HtmlRegex = new Regex(@"</?([a-z]+[1-6]?)", RegexOptions.IgnoreCase);
		static HashSet<string> HtmlTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "a", "abbr", "acronym", "address", "applet", "area", "article", "aside", "audio", "b", "base", "bdi", "bdo", "big", "blockquote", "body", "br", "button", "canvas", "caption", "center", "cite", "code", "col", "colgroup", "command", "datalist", "dd", "del", "details", "dfn", "dir", "div", "dl", "dt", "em", "embed", "fieldset", "figcaption", "figure", "font", "footer", "form", "frame", "h1", "h2", "h3", "h4", "h5", "h6", "head", "header", "hgroup", "hr", "html", "i", "iframe", "img", "input", "ins", "isindex", "kbd", "keygen", "label", "legend", "li", "link", "map", "mark", "menu", "meta", "meter", "nav", "noscript", "object", "ol", "optgroup", "option", "output", "p", "param", "pre", "progress", "q", "rp", "rt", "ruby", "s", "samp", "script", "section", "select", "small", "source", "span", "strike", "strong", "style", "sub", "summary", "sup", "table", "tbody", "td", "textarea", "tfoot", "th", "thead", "time", "title", "tr", "track", "tt", "u", "ul", "var", "video", "wbr" };

		public static string ToUrlFriendly(this string value) =>
			value != null ? Regex.Replace(value.ToLower(), @"[^A-Za-z0-9_\.~]+", "-") : null;

		public static string ToStringInvariant(this Enum enumeration)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}", enumeration);
		}

		public static string ToStringInvariant(this object obj)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}", obj.ToString());
		}

		/// <summary>
		/// Removes all special characters from the string.
		/// </summary>
		/// <param name="input"></param>
		/// <returns>The adjusted string.</returns>
		public static string RemoveAllSpecialCharacters(this string input)
		{
			var sb = new StringBuilder(input.Length);
			foreach (var c in input.Where(c => Char.IsLetterOrDigit(c))) {
				sb.Append(c);
			}
			return sb.ToString();
		}

		public static string GetEnvOrDefault(this string keyName, string defaultValue = null)
		{
			var value = Environment.GetEnvironmentVariable(keyName);
			if (string.IsNullOrWhiteSpace(value))
			{
				if (defaultValue == null)
				{
					throw new ArgumentException($"environment variable {keyName} is not found");
				}

				value = defaultValue;
			}

			return value;
		}

		public static TReturnValue Deserialize<TReturnValue>(this string jsonString)
		{
			var returnValue = default(TReturnValue);
			if (!string.IsNullOrWhiteSpace(jsonString))
			{
				returnValue = JsonConvert.DeserializeObject<TReturnValue>(jsonString);
			}

			return returnValue;
		}

		public static bool EqualTo(this string value, string stringToCompare)
		{
			return string.Equals(value, stringToCompare, StringComparison.InvariantCultureIgnoreCase);
		}

		public static string ToHmacSha256(this string message, string secret)
		{
			var encoding = new ASCIIEncoding();
			var keyBytes = encoding.GetBytes(secret);
			var messageBytes = encoding.GetBytes(message);
			var cryptographer = new HMACSHA256(keyBytes);

			var bytes = cryptographer.ComputeHash(messageBytes);

			return BitConverter.ToString(bytes).Replace("-", "").ToLower();
		}

		public static TEnum ToValidEnum<TEnum>(this string enumName)
		{
			var enumValue = default(TEnum);
			if (!string.IsNullOrWhiteSpace(enumName))
			{
				var validNames = Enum.GetNames(typeof(TEnum));
				var nameFound = string.Empty;
				foreach (var item in validNames)
				{
					if (item.EqualTo(enumName))
					{
						nameFound = item;
						break;
					}
				}

				if (!string.IsNullOrWhiteSpace(nameFound))
				{
					enumValue = (TEnum) Enum.Parse(typeof(TEnum), nameFound);
				}
				else
				{
					throw new ArgumentOutOfRangeException($"{enumName} is not a valid enum.");
				}
			}

			return enumValue;
		}

		/// <summary>
		/// Truncates the string to a specified length and replace the truncated to a ...
		/// </summary>
		/// <param name="value">string that will be truncated</param>
		/// <param name="maxLength">
		/// total length of characters to maintain before the truncate happens
		/// </param>
		/// <param name="replaceTruncatedCharWithEllipsis">
		/// Flag whether to replace with ellipsis.
		/// </param>
		/// <returns>The truncated string</returns>
		public static string Truncate(this string value, int maxLength, bool replaceTruncatedCharWithEllipsis = false)
		{
			if (replaceTruncatedCharWithEllipsis && maxLength <= 3)
				throw new ArgumentOutOfRangeException(nameof(maxLength),
					"maxLength should be greater than three when replacing with an ellipsis.");

			if (string.IsNullOrWhiteSpace(value))
			{
				return string.Empty;
			}

			if (replaceTruncatedCharWithEllipsis &&
			    value.Length > maxLength)
			{
				return value.Substring(0, maxLength - 3) + "...";
			}

			return value.Substring(0, Math.Min(value.Length, maxLength));
		}

		/// <summary>
		/// Converts the input string to title case.
		/// </summary>
		/// <param name="s">The input string.</param>
		/// <returns>The title case string.</returns>
		public static string ToTitleCase(this string s) =>
			CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());

		/// <summary>
		/// Converts the input string to camelCase
		/// </summary>
		/// <param name="s">The InputString</param>
		/// <returns>The camelCaseString</returns>
		public static string ToCamelCase(this string s) =>
			s?.Length > 0 ? Char.ToLowerInvariant(s[0]) + s.Substring(1) : s;

		/// <summary>
		/// Returns whether string is empty or not
		/// </summary>
		/// <param name="s">The InputString</param>
		/// <returns>True when string is null or whitespace, false otherwise</returns>
		public static bool IsEmpty(this string s) => string.IsNullOrWhiteSpace(s);

		/// <summary>
		/// Returns whether string is not empty
		/// </summary>
		/// <param name="s">The InputString</param>
		/// <returns>False when string is null or whitespace, true otherwise</returns>
		public static bool IsNotEmpty(this string s) => !IsEmpty(s);

		public static async Task<Stream> AsStream(this string input)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			await writer.WriteAsync(input);
			await writer.FlushAsync();
			stream.Position = 0;
			return stream;
		}

		public static IList<T> ToEnumList<T>(this string input, bool failOnMistmach = false, string separator = ",")
		{
			var result = new List<T>();
			if (input != null)
			{
				var enumType = typeof(T);
				var chunks = input.Split(separator);
				foreach (var chunk in chunks)
				{
					if (Enum.TryParse(enumType, chunk, true, out var item) && Enum.IsDefined(enumType, item))
					{
						result.Add((T) item);
					}
					else
					{
						if (failOnMistmach)
						{
							throw new ArgumentOutOfRangeException(
								$"{chunk} is not a valid value of {typeof(T).Name} enum");
						}
					}
				}
			}

			return result;
		}

		public static string ToBase64(this string plainText)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

		/// <summary>
		/// Split string by separator and trims each part of the result
		/// </summary>
		/// <param name="value">String to split</param>
		/// <param name="separator">Separator to split by</param>
		/// <returns>Trimmed chunks of the input string</returns>
		public static string[] SplitAndTrim(this string value, string separator)
		{
			if (value != null)
			{
				return value.Split(separator, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim())
					.ToArray();
			}
			else
			{
				return new string[] {"empty"};
			}
		}

		/// <summary>
		/// Removes the whitespace from the string.
		/// </summary>
		/// <param name="value">The input string</param>
		/// <returns>String without whitespace.</returns>
		public static string RemoveWhitespace(this string value) {
			return string.Join("", value.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
		}

		/// <summary>
		/// Strips html tags from the text.
		/// </summary>
		/// <param name="value">The input value.</param>
		/// <param name="useRegularExpression">Determines whether to use regular expression.</param>
		/// <param name="keepLinks">Determines whether to keep the anchor tags.</param>
		/// <returns>The text without the html tags.</returns>
		public static string StripHtml(this string value, bool useRegularExpression = true, bool keepLinks = false)
		{
			if (useRegularExpression && keepLinks)
			{
				throw new ArgumentException("With regular expression links stripping, tags cannot be maintained");
			}

			if (useRegularExpression)
			{
				return ServiceStackStringExtensions.StripHtml(value);
			}

			// Use an html parser here to remove all tags except the selected ones

			var allowedTags = new List<string>();
			var allowedAttributes = new List<string>();

			if (keepLinks)
			{
				allowedTags.Add("a");
				allowedAttributes.Add("href");
			}

			try
			{
				// Encode non-HTML before sanitizing
				value = HtmlRegex.Replace(value, m =>
				{
					var tagName = m.Groups[1].Value;
					if (!HtmlTags.Contains(tagName))
						return "&lt;" + m.Value.Substring(1);
					return m.Value;
				});

				var sanitizer = new HtmlSanitizer(allowedTags: allowedTags, allowedAttributes: allowedAttributes,
					allowedSchemes: new string[] { }, uriAttributes: new string[] { },
					allowedCssProperties: new string[] { }) {KeepChildNodes = true};

				if (keepLinks)
				{
					// Force links to always open in a new window
					sanitizer.PostProcessNode += (s, e) =>
						(e.Node as IHtmlAnchorElement)?.SetAttribute("target", "_blank");
				}

				return sanitizer.Sanitize(value).Replace("\n", "<br>");
			}
			catch (Exception)
			{
				// Provide a fallback for the method in case of error parsing the html document
				return ServiceStackStringExtensions.StripHtml(value);
			}
		}
	}
}