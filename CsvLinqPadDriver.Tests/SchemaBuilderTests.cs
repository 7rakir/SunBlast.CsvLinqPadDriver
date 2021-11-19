using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using CsvLinqPadDriver.Tests.Utils;
using LINQPad.Extensibility.DataContext;

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
		
		[Test]
		public void GenerateSchemaForModelWithCustomPrefix_SchemaShouldBeProperlySegmented()
		{
			var dataDescription = new DataDescription(new[] { "Header1" }, true);

			var fileDescriptions = new[] {
				new FileDescription("voipFirst", 1, DateTime.MinValue),
				new FileDescription("netflowUncategorized", 3, DateTime.MinValue),
				new FileDescription("VoIPSecond", 5, DateTime.MinValue),
			};

			var fileModels = fileDescriptions.Select(f => new FileModel(f, dataDescription));

			var result = DataGeneration.GetSchema(fileModels).ToArray();

			Assert.That(result[0].Text, Is.EqualTo("netflowUncategorized"));
			Assert.That(result[1].Text, Is.EqualTo("voip (2)"));
			Assert.That(result[1].Children[0].Text, Is.EqualTo("voipFirst"));
			Assert.That(result[1].Children[1].Text, Is.EqualTo("VoIPSecond"));
		}
		
		[Test]
		public void InvalidClassNameCharactersInName_GenerateSchemaForModel_SchemaShouldBeProperlySegmented()
		{
			var dataDescription = new DataDescription(new[] { "Header1" }, true);

			var fileDescriptions = new[] {
				new FileDescription("C:/directory/-filename1.extension", 1, DateTime.MinValue),
				new FileDescription("C:/directory/^filename2.extension", 2, DateTime.MinValue),
				new FileDescription("C:/directory/.filename3.extension", 3, DateTime.MinValue),
				new FileDescription("C:/directory/--filename4.extension", 4, DateTime.MinValue),
				new FileDescription("C:/directory/--filename5.extension", 5, DateTime.MinValue),
				new FileDescription("C:/directory/-pref-filename6.extension", 6, DateTime.MinValue),
				new FileDescription("C:/directory/-pref-filename7.extension", 7, DateTime.MinValue),
				new FileDescription("C:/directory/pref--filename8.extension", 8, DateTime.MinValue),
				new FileDescription("C:/directory/pref--filename9.extension", 9, DateTime.MinValue),
				new FileDescription("C:/directory/  filename 10  .extension", 9, DateTime.MinValue)
			};

			var fileModels = fileDescriptions.Select(f => new FileModel(f, dataDescription));

			var result = DataGeneration.GetSchema(fileModels).ToArray();

			Assert.That(result[0].Text, Is.EqualTo("__filename_10__"));
			Assert.That(result[1].Text, Is.EqualTo("__filename4"));
			Assert.That(result[2].Text, Is.EqualTo("__filename5"));
			Assert.That(result[3].Text, Is.EqualTo("_filename1"));
			Assert.That(result[4].Text, Is.EqualTo("_filename2"));
			Assert.That(result[5].Text, Is.EqualTo("_filename3"));
			Assert.That(result[6].Text, Is.EqualTo("_pref_filename6"));
			Assert.That(result[7].Text, Is.EqualTo("_pref_filename7"));
			Assert.That(result[8].Text, Is.EqualTo("pref (2)"));
			Assert.That(result[8].Children[0].Text, Is.EqualTo("pref__filename8"));
			Assert.That(result[8].Children[1].Text, Is.EqualTo("pref__filename9"));
		}

		[TestCase("C:/directory/1filename.extension", "_1filename")]
		[TestCase("C:/directory/_filename.extension", "_filename")]
		public void InvalidClassNameCharactersInName_GenerateSchemaForModel_SchemaShouldBeProperlyGenerated(
			string filePath, string expectedName)
		{
			var dataDescription = new DataDescription(new[] { "Header1" }, true);
		
			var fileDescriptions = new[] {
				new FileDescription(filePath, 1, DateTime.MinValue)
			};
		
			var fileModels = fileDescriptions.Select(f => new FileModel(f, dataDescription));
		
			var result = DataGeneration.GetSchema(fileModels).ToArray();
		
			Assert.That(result[0].Text, Is.EqualTo(expectedName));
		}
	}
}