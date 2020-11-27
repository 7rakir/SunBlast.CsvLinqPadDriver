using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace Tarydium.CSVLINQPadDriver.Tests
{
	public class Tests
	{
		[Test]
		public void Test1()
		{
			var tree = new SyntaxTreeGenerator("TestNamespace", "ContextClass").GetSyntaxTree(Enumerable.Empty<DataFile>());

			var result = tree.GetRoot().NormalizeWhitespace().ToFullString();

			Console.WriteLine(result);
		}
	}
}