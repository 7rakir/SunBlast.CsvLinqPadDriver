using System;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace Tarydium.CSVLINQPadDriver.Tests
{
	public class UnitTest1
	{
		[Test]
		public void Test1()
		{
			var schema = new[]
			{
				new FileModel("Model.extension", new[] {"Header1", "Header-2"})
			};

			var syntaxTreeGenerator = new SyntaxTreeGenerator("ContextClass");
			foreach (var fileModel in schema)
			{
				syntaxTreeGenerator.AddTable(fileModel);
			}
			var tree = syntaxTreeGenerator.Build("TestNamespace");

			var result = tree.GetRoot().NormalizeWhitespace().ToFullString();

			Console.WriteLine(result);
		}
	}
}