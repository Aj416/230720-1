using System;
using Xunit;
using Tigerspike.Solv.Core.Extensions;


namespace Tigerspike.Solv.Core.Tests.Extensions.StringExtensionsTests
{
	public class StripHtmlTests
	{
		[Fact]
		public void StripAllUsingRegularExpression()
		{
			string html =
				@"<div>a div</div>
        A
        <script>alert('hello');</script>
        B
        <iframe src=""http://www.west-wind.com"" class=""iframeclass""></iframe>
        C
        <a href=""javascript:alert('xss');"" class='hoverbutton' />
        <br/>
        D
        <img src=""javascript:alert('xss')"" class='hoverbutton' />
        E
        <img src=`javascript:alert('xss')` class='hoverbutton' />
        F
        <img src='http://www.west-wind.com/images/new.gif' class='hoverbutton' />
        G
        <div onclick=""alert('xss')"" class='test'>
        <div style=""color: expression(0)"" >
        </div>
<span>
        </div>
</div>
";
			var expected = "adivAalert('hello');BCDEFG";
			var result = StringExtensions.StripHtml(html, true);

			Assert.Equal(expected, result.RemoveWhitespace().Replace("\n", ""));
		}

		[Fact]
		public void StripAllUsingNonRegularExpression()
		{
			string html =
				@"<div><span>a div</span></div>
        A
        <script>alert('hello');</script>
        B
        <iframe src=""http://www.west-wind.com"" class=""iframeclass""></iframe>
        C
        <br/>
        D
        <img src=""javascript:alert('xss')"" class='hoverbutton' />
        E
        <img src=`javascript:alert('xss')` class='hoverbutton' />
        F
        <img src='http://www.west-wind.com/images/new.gif' class='hoverbutton' />
        G
        <a href=""javascript:alert('xss');"" class='hoverbutton'></a>
		H
        <div onclick=""alert('xss')"" class='test'>
        <div style=""color: expression(0)"" >
        </div>
<span>
        </div>
</div>
";
			var expected = "adiv<br>A<br>alert('hello');<br>B<br><br>C<br><br>D<br><br>E<br><br>F<br><br>G<br><br>H<br><br><br><br><br><br><br>";
			var result = StringExtensions.StripHtml(html, false);

			Assert.Equal(expected, result.RemoveWhitespace());
		}

		[Fact]
		public void KeepLinks()
		{
			string html =
				@"<strong>Hello</strong> this is a text <a href=""https://www.google.com"">Google</a>";

			var expected = @"Hello this is a text <a href=""https://www.google.com"" target=""_blank"">Google</a>";
			var result = StringExtensions.StripHtml(html, false, true);

			Assert.Equal(expected, result);
		}

		[Fact]
		public void KeepTextIntactWithoutHtml()
		{
			string nonHtml =
				@"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis at pellentesque erat. Aliquam nunc velit, molestie quis ultricies eget, tincidunt eget felis. Praesent rhoncus vulputate est, vitae semper augue tempor ut. Pellentesque leo tortor, commodo vel nisl vitae, gravida vehicula urna. Suspendisse vehicula purus ut sem pulvinar suscipit. Pellentesque suscipit venenatis massa, ut placerat urna dignissim vel. Nulla tempor scelerisque lorem, sit amet ullamcorper elit dignissim vel. Sed ut felis ut metus lobortis mollis. Vestibulum urna libero, bibendum non est vitae, malesuada vestibulum ante. Proin congue metus risus, et porta tortor lacinia sed. Duis et enim dui. Vestibulum non odio aliquet, eleifend ligula tincidunt, lacinia ligula. ";

			var result = StringExtensions.StripHtml(nonHtml, false, true);

			Assert.Equal(nonHtml, result);
		}

		[Fact]
		public void WorksWithInvalidHtml()
		{
			string nonHtml =
				@"Lorem ipsum dolor sit amet, <consectetur adipiscing elit. Duis at Aliquam nunc velit, <molestie> ultricies eget,";

			string expected =
				@"Lorem ipsum dolor sit amet, &lt;consectetur adipiscing elit. Duis at Aliquam nunc velit, &lt;molestie&gt; ultricies eget,";

			var result = StringExtensions.StripHtml(nonHtml, false, true);

			Assert.Equal(expected, result);
		}

		[Fact]
		public void WorksWithInvalidTags()
		{
			string nonHtml =
				@"< Lorem ipsum <dolor sit amet, <consectetur] [adipiscing elit. Duis at Aliquam nunc velit, <molestie> ultricies eget,";

			string expected =
				@"&lt; Lorem ipsum &lt;dolor sit amet, &lt;consectetur] [adipiscing elit. Duis at Aliquam nunc velit, &lt;molestie&gt; ultricies eget,";

			var result = StringExtensions.StripHtml(nonHtml, false, true);

			Assert.Equal(expected, result);
		}
	}
}
