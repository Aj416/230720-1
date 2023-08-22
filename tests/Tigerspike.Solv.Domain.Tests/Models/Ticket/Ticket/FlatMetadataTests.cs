using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Tigerspike.Solv.Domain.Models;
using Xunit;

namespace Tigerspike.Solv.Domain.Tests.Models
{
	public class FlatMetadataTests
	{
		[Fact]
		public void ShouldReturnNullWhenThereWasNoMetadata()
		{
			// Arrange
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Metadata, new List<TicketMetadataItem>())
				.Build();			

			// Act
			var result = ticket.FlatMetadata;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void ShouldReturnSingleKeyValuePairWhenMetadataContainsOneItem()
		{
			// Arrange
			var metadata = new [] {	GetMetadataItem("SerialNumber", "123-XYZ") };
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Metadata, metadata.ToList())
				.Build();

			// Act
			var result = ticket.FlatMetadata;

			// Assert
			Assert.Equal("SerialNumber:123-XYZ", result);
		}

		[Fact]
		public void ShouldReturnSeparatedKeyValuePairsWhenMetadataContainsMultipleItems()
		{
			// Arrange
			var metadata = new[]
			{
				GetMetadataItem("SerialNumber", "123-XYZ"),
				GetMetadataItem("Model", "X1"),
				GetMetadataItem("Company", "Tesla"),
			};
			var ticket = Builder<Ticket>
				.CreateNew()
				.With(x => x.Metadata, metadata.ToList())
				.Build();			

			// Act
			var result = ticket.FlatMetadata;

			// Assert
			Assert.Equal("SerialNumber:123-XYZ|Model:X1|Company:Tesla", result);
		}		

		private TicketMetadataItem GetMetadataItem(string key, string value)
		{
			return Builder<TicketMetadataItem>
				.CreateNew()
				.With(x => x.Key, key)
				.With(x => x.Value, value)
				.Build();
		}

	}
}