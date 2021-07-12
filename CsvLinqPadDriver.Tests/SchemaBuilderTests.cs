using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace CsvLinqPadDriver.Tests
{
	[TestFixture]
	public class SchemaBuilderTests
	{
		[Test]
		public void GenerateSchemaForLargeModel_ShouldBeFast()
		{
			var schemaModel = DataGeneration.GetLargeSchemaModel().ToArray();

			var watch = Stopwatch.StartNew();

			_ = DataGeneration.GetSchema(schemaModel).ToArray();

			watch.Stop();

			Console.WriteLine(watch.Elapsed);
			
			Assert.That(watch.ElapsedMilliseconds, Is.LessThan(10));
		}

		[Test]
		public void GenerateSchemaForModel_SchemaShouldBeProperlySegmented()
		{
			var singleHeader = new[] {"Header1"};

			var fileModels = new[]
			{
				new FileModel("CCC", singleHeader),
				new FileModel("A_1", singleHeader),
				new FileModel("BBB", singleHeader),
				new FileModel("C_1", singleHeader),
				new FileModel("DDD", singleHeader),
				new FileModel("A_2", singleHeader),
			};

			var result = DataGeneration.GetSchema(fileModels).ToArray();

			Assert.That(result[0].Text, Is.EqualTo("A (2)"));
			Assert.That(result[0].Children[0].Text, Is.EqualTo("A_1"));
			Assert.That(result[0].Children[1].Text, Is.EqualTo("A_2"));
			Assert.That(result[1].Text, Is.EqualTo("BBB"));
			Assert.That(result[2].Text, Is.EqualTo("C_1"));
			Assert.That(result[3].Text, Is.EqualTo("CCC"));
			Assert.That(result[4].Text, Is.EqualTo("DDD"));
		}
	}
}