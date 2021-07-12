using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace CsvLinqPadDriver.Tests
{
	[TestFixture]
	public class SyntaxTreeBuilderTests
	{
		[Test]
		public void GenerateSyntaxTree()
		{
			var schema = new[]
			{
				new FileModel("Model.extension", new[] {"Header1", "Header-2"})
			};
			
			var tree = DataGeneration.GetSyntaxTree(schema);

			var result = tree.GetRoot().NormalizeWhitespace().ToFullString();

			const string expectedOutput = @"using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvParser;

namespace TestNamespace
{
    public class TestContextClass
    {
        public IEnumerable<Model> Model => CsvReader.ReadFile<Model>(""Model.extension"")}

    public class Model
    {
        public string Header1 { get set }

        public string Header-2 { get set }
    }
}";

			Assert.AreEqual(expectedOutput, result);
		}
	}
}