using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Tarydium.CSVLINQPadDriver.Tests
{
	public class SchemaBuilderTests
	{
		[Test]
		public void Test3()
		{
			var schemaModel = GetLargeSchemaModel().ToArray();

			var watch = Stopwatch.StartNew();

			_ = GetSchema(schemaModel).ToArray();

			watch.Stop();

			Console.WriteLine(watch.Elapsed);
			
			Assert.That(watch.ElapsedMilliseconds, Is.LessThan(10));
		}
		
		[Test]
		public void Test5()
		{
			var schemaModel = GetLargeSchemaModel().ToArray();
			var syntaxTree = _ = GetSyntaxTree(schemaModel);

			var assembly = new AssemblyName("AssemblyName");
			
			var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
			
			var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

			var references = new string[]{}
				.Append(typeof(CsvParser.CsvReader).Assembly.Location)
				.Append(typeof(object).Assembly.Location)
				.Append(typeof(Enumerable).Assembly.Location)
				.Append(typeof(GCSettings).Assembly.Location)
				.Append(typeof(ICollection).Assembly.Location)
				.Append(Path.Combine(assemblyPath, "System.Runtime.dll"))
				.Select(x => MetadataReference.CreateFromFile(x));

			var watch = Stopwatch.StartNew();

			var compilation = CSharpCompilation.Create(assembly.FullName).AddSyntaxTrees(syntaxTree).AddReferences(references).WithOptions(options);

			using var stream = new MemoryStream();
			
			var result = compilation.Emit(stream);

			watch.Stop();

			Console.WriteLine(watch.Elapsed);
			
			var errorMessage = result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()).Take(5);
			
			Assert.That(result.Success, Is.True, String.Join(Environment.NewLine, errorMessage));
			
			Assert.That(watch.ElapsedMilliseconds, Is.LessThan(3000));
		}

		[Test]
		public void Test4()
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

			var result = GetSchema(fileModels).ToArray();

			Assert.That(result[0].Text, Is.EqualTo("A (2)"));
			Assert.That(result[0].Children[0].Text, Is.EqualTo("A_1"));
			Assert.That(result[0].Children[1].Text, Is.EqualTo("A_2"));
			Assert.That(result[1].Text, Is.EqualTo("BBB"));
			Assert.That(result[2].Text, Is.EqualTo("C_1"));
			Assert.That(result[3].Text, Is.EqualTo("CCC"));
			Assert.That(result[4].Text, Is.EqualTo("DDD"));
		}

		private static IEnumerable<ExplorerItem> GetSchema(IEnumerable<FileModel> schemaModel)
		{
			var builder = new SchemaBuilder();
			foreach (var fileModel in schemaModel)
			{
				builder.AddModel(fileModel);
			}
			return builder.BuildSchema();
		}
		
		private static SyntaxTree GetSyntaxTree(IEnumerable<FileModel> schemaModel)
		{
			var syntaxTreeBuilder = new SyntaxTreeBuilder("MyClass");
			foreach (var fileModel in schemaModel)
			{
				syntaxTreeBuilder.AddModel(fileModel);
			}
			return syntaxTreeBuilder.Build("MyNameSpace");
		}

		private static IEnumerable<FileModel> GetLargeSchemaModel()
		{
			for (int i = 0; i < 1000; i++)
			{
				yield return new FileModel($"File{i:000}.csv",
					new[]
					{
						"Header1", "Header2", "Header3", "Header4", "Header5", "Header6", "Header7", "Header8", "Header9"
					});
			}
		}
	}
}