using System;
using CsvParser;
using NUnit.Framework;

namespace Tarydium.CSVLINQPadDriver.Tests
{
	public class UnitTest2
	{
		[Test]
		public void Test2()
		{
			var path = @"C:\repos\Tarydium.CSVLINQPadDriver\Tarydium.CSVLINQPadDriver.Tests\Tested.csv";
			var data = CsvReader.ReadFile<Data>(path);

			var expected = new[]
			{
				new Data { Column1 = "Row1Column1", Column2 = "Row1Column2", Column3 = "Row1Column3" },
				new Data { Column1 = "Row2Column1", Column2 = "Row2Column2", Column3 = $"Row2{Environment.NewLine}Column3" },
				new Data { Column1 = "Row3Column1", Column2 = "Row3Column2", Column3 = "Row3Column3" },
				new Data { Column1 = "Row4,Column1", Column2 = "Row4,Column2", Column3 = $"Row4,{Environment.NewLine}Column3" }
			};

			CollectionAssert.AreEqual(expected, data);
		}

		public class Data
		{
			public string Column1 { get; set; }
			public string Column2 { get; set; }
			public string Column3 { get; set; }

			public override string ToString()
			{
				return $"{Column1},{Column2},{Column3}";
			}

			public override bool Equals(object? obj)
			{
				if (obj is Data data)
				{
					return Column1 == data.Column1 && Column2 == data.Column2 && Column3 == data.Column3;
				}

				return false;
			}
		}
	}
}