using NUnit.Framework;
using System;
using CsvLinqPadDriver.Csv;
using CsvLinqPadDriver.Tests.Utils;

namespace CsvLinqPadDriver.Tests
{
	[TestFixture]
	public class CsvReaderTests
	{
		[Test]
		public void CsvWithDataSplitAcrossLines_ShouldBeProperlyParsed()
		{
			string path = TestData.Path<CsvReaderTests>("Normal.csv");
			var data = CsvReader.ReadFile<Data>(path);

			var expected = new[]
			{
				new Data("Row1Column1", "Row1Column2", "Row1Column3"),
				new Data("Row2Column1", "Row2Column2", $"Row2{Environment.NewLine}Column3"),
				new Data("Row3Column1", "Row3Column2", "Row3Column3"),
				new Data("Row4,Column1", "Row4,Column2", $"Row4,{Environment.NewLine}Column3")
			};

			CollectionAssert.AreEqual(expected, data);
		}

		private record Data(string Column1, string Column2, string Column3);
	}
}