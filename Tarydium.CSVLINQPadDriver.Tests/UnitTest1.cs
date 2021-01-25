using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace Tarydium.CSVLINQPadDriver.Tests
{
	public class UnitTest1
	{
		[Test]
		public void Test1()
		{
			var reader = new MockedReader();

			var schema = reader.GetSchema("Model.extension");

			var tree = new SyntaxTreeGenerator().GetSyntaxTree("TestNamespace", "ContextClass", schema);

			var result = tree.GetRoot().NormalizeWhitespace().ToFullString();

			Console.WriteLine(result);
		}

		private class MockedReader : ISchemaReader
		{
			public IEnumerable<FileModel> GetSchema(string path)
			{
				return new[]
				{
					new FileModel(path, new[] {"Header1", "Header-2"})
				};
			}
		}
	}
}