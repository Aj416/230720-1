using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Tigerspike.Solv.Core.Mvc
{
	/// <summary>
	/// Hosting Environment Extensions
	/// </summary>
	public static class HostingEnvironmentExtensions
	{
		/// <summary>
		/// Is Local Environment
		/// </summary>
		/// <param name="hostingEnvironment"></param>
		/// <returns></returns>
		public static bool IsLocal(this IWebHostEnvironment hostingEnvironment) => hostingEnvironment.IsEnvironment("local");

		/// <summary>
		/// Is Docker Environment
		/// </summary>
		/// <param name="hostingEnvironment"></param>
		/// <returns></returns>
		public static bool IsDocker(this IWebHostEnvironment hostingEnvironment) => hostingEnvironment.IsEnvironment("docker");

		/// <summary>
		/// Is Dev Environment
		/// </summary>
		/// <param name="hostingEnvironment"></param>
		/// <returns></returns>
		public static bool IsDev(this IWebHostEnvironment hostingEnvironment) => hostingEnvironment.IsEnvironment("dev");

		/// <summary>
		/// Is SIT Environment
		/// </summary>
		/// <param name="hostingEnvironment"></param>
		/// <returns></returns>
		public static bool IsSit(this IWebHostEnvironment hostingEnvironment) => hostingEnvironment.IsEnvironment("sit");

		/// <summary>
		/// Is UAT Environment
		/// </summary>
		/// <param name="hostingEnvironment"></param>
		/// <returns></returns>
		public static bool IsUat(this IWebHostEnvironment hostingEnvironment) => hostingEnvironment.IsEnvironment("uat");

		/// <summary>
		/// Is Prod Environment
		/// </summary>
		/// <param name="hostingEnvironment"></param>
		/// <returns></returns>
		public static bool IsProd(this IWebHostEnvironment hostingEnvironment) => hostingEnvironment.IsEnvironment("prod");
	}
}