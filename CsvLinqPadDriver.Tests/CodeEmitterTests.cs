using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvLinqPadDriver.Tests.Utils;

namespace CsvLinqPadDriver.Tests
{
	[TestFixture]
	public class CodeEmitterTests
	{
		[Test]
		public void CodeEmittingShouldBeReasonablySlow()
		{
			var schemaModel = DataGeneration.GetLargeSchemaModel();
			var syntaxTree = DataGeneration.GetSyntaxTree(schemaModel);

			var assembly = new AssemblyName("AssemblyName");

			EmitResult? result;

			using (DurationAssert.Milliseconds(2000))
			{
				var compilation = CodeEmitter.GetCompilation(assembly, syntaxTree);

				using var stream = new MemoryStream();

				result = compilation.Emit(stream);
			}

			var errorMessage = result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()).Take(5);

			Assert.That(result.Success, Is.True, String.Join(Environment.NewLine, errorMessage));
		}
	}
}