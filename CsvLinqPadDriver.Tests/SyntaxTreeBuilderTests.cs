using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System;

namespace CsvLinqPadDriver.Tests
{
	[TestFixture]
	public class SyntaxTreeBuilderTests
	{
		[Test]
		public void GenerateSyntaxTree()
		{
			var file = new FileDescription("Model.extension", 0, DateTime.MinValue);

			var data = new DataDescription(new[] { "Header1", "Header-2" }, true);

			var schema = new[]
			{
				new FileModel(file, data)
			};

			var tree = DataGeneration.GetSyntaxTree(schema);

			var result = tree.GetRoot().NormalizeWhitespace().ToFullString();

			const string expectedOutput = @"using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvLinqPadDriver.DataExtensions;
using CsvParser;

namespace TestNamespace
{
    public class TestContextClass
    {
        public IEnumerable<Model> Model => CsvReader.ReadFile<Model>(""Model.extension"")}

    public class Model
    {
        public string Header1;
        public string Header-2;
    }

    public static class ModelExtensions
    {
        public static IEnumerable<Nodes> WhereDelayed(this IEnumerable<Nodes> enumerable, DateTime timeOfGatheringDiagnostics) => enumerable.WhereDelayed<Nodes>(timeOfGatheringDiagnostics)public static IEnumerable<CortexDocument> Parse(this IEnumerable<Cortex_Documents> enumerable) => enumerable.ParseCortex()}
}";

			Assert.AreEqual(expectedOutput, result);
		}
	}
}