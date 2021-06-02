using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Tarydium.CSVLINQPadDriver.Tests
{
	public class UnitTest3
	{
		[Test]
		public void Test3()
		{
			var schema = GetLargeSchema().ToArray();

			var watch = Stopwatch.StartNew();

			TreeGenerator.GetTree(schema);

			watch.Stop();

			Console.WriteLine(watch.Elapsed);
		}

		[Test]
		public void Test4()
		{
			var singleHeader = new[] {"Header1"};

			var schema = new[]
			{
				new FileModel("CCC", singleHeader),
				new FileModel("A_1", singleHeader),
				new FileModel("BBB", singleHeader),
				new FileModel("C_1", singleHeader),
				new FileModel("DDD", singleHeader),
				new FileModel("A_2", singleHeader),
			};

			var result = TreeGenerator.GetTree(schema);

			Assert.That(result[0].Text, Is.EqualTo("A"));
			Assert.That(result[0].Children[0].Text, Is.EqualTo("1"));
			Assert.That(result[0].Children[1].Text, Is.EqualTo("2"));
			Assert.That(result[1].Text, Is.EqualTo("BBB"));
			Assert.That(result[2].Text, Is.EqualTo("C_1"));
			Assert.That(result[3].Text, Is.EqualTo("CCC"));
			Assert.That(result[4].Text, Is.EqualTo("DDD"));
		}

		private static IEnumerable<FileModel> GetLargeSchema()
		{
			for (int i = 0; i < 1000; i++)
			{
				yield return new FileModel($"File{i:000}.csv",
					new[]
					{
						"Header1", "Header2", "Header3", "Header4", "Header5", "Header6", "Header7", "Header8", "Header9"
					});
			}
		}


	}
}