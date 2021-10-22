using NUnit.Framework;
using System;
using System.Linq;

namespace CsvLinqPadDriver.Tests
{
	[TestFixture]
	public class SchemaBuilderTests
	{
		[Test]
		public void GenerateSchemaForLargeModel_ShouldBeFast()
		{
			var schemaModel = DataGeneration.GetLargeSchemaModel().ToArray();

			using (DurationAssert.StartNew(20))
			{
				_ = DataGeneration.GetSchema(schemaModel).ToArray();
			}
		}

		[Test]
		public void GenerateSchemaForModel_SchemaShouldBeProperlySegmented()
		{
			var dataDescription = new DataDescription(new[] { "Header1" }, true);

			var fileDescriptions = new[] {
				new FileDescription("CCC", 1, DateTime.MinValue),
				new FileDescription("A_1", 2, DateTime.MinValue),
				new FileDescription("BBB", 3, DateTime.MinValue),
				new FileDescription("C_1", 4, DateTime.MinValue),
				new FileDescription("DDD", 5, DateTime.MinValue),
				new FileDescription("A_2", 6, DateTime.MinValue)
			};

			var fileModels = fileDescriptions.Select(f => new FileModel(f, dataDescription));

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