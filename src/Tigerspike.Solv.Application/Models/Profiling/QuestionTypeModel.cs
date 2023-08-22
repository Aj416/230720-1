namespace Tigerspike.Solv.Application.Models.Profile
{
	public class QuestionTypeModel
	{
		/// <summary>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// </summary>
		public bool IsMultiChoice { get; set; }

		/// <summary>
		/// </summary>
		public bool IsSlider { get; set; }

		/// <summary>
		/// </summary>
		public bool IsAllRequired { get; set; } = false;
	}
}