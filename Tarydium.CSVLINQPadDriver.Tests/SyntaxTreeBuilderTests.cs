using System;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace Tarydium.CSVLINQPadDriver.Tests
{
	public class SyntaxTreeBuilderTests
	{
		[Test]
		[Explicit("Displays the output of building the syntax tree. Unless it fails during the build, it does not have a testing value.")]
		public void GenerateSyntaxTree()
		{
			var schema = new[]
			{
				new FileModel("Model.extension", new[] {"Header1", "Header-2"})
			};

			var syntaxTreeGenerator = new SyntaxTreeBuilder("ContextClass");
			foreach (var fileModel in schema)
			{
				syntaxTreeGenerator.AddModel(fileModel);
			}
			var tree = syntaxTreeGenerator.Build("TestNamespace");

			var result = tree.GetRoot().NormalizeWhitespace().ToFullString();

			Console.WriteLine(result);
		}
	}
}