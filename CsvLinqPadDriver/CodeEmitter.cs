using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvLinqPadDriver.Csv;
using LINQPad.Extensibility.DataContext;

namespace CsvLinqPadDriver
{
	internal static class CodeEmitter
	{
		public static void Emit(SyntaxTree syntaxTree, AssemblyName assembly)
		{
			var compilation = GetCompilation(assembly, syntaxTree);

			using var fileStream = File.OpenWrite(assembly.CodeBase!);

			var result = compilation.Emit(fileStream);

			if (result.Success)
			{
				return;
			}
			
			LogError(result);
			throw new InvalidOperationException(@"Emitting compilation failed. See '%localappdata%\LINQPad\Logs.LINQPad6\SunBlast.CsvLinqPadDriver.log' for more information.");
		}

		internal static Compilation GetCompilation(AssemblyName assembly, SyntaxTree syntaxTree)
		{
			var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

			var references = GetReferences();

			return CSharpCompilation
				.Create(assembly.FullName)
				.WithOptions(options)
				.AddReferences(references)
				.AddSyntaxTrees(syntaxTree);
		}
		
		private static IEnumerable<MetadataReference> GetReferences()
		{
			return DataContextDriver
				.GetCoreFxReferenceAssemblies()
				.Append(typeof(CsvReader).Assembly.Location)
				.Select(x => MetadataReference.CreateFromFile(x));
		}

		private static void LogError(EmitResult result)
		{
			var message = string.Join(Environment.NewLine,
				result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()));

			DynamicDriver.WriteToLog(message);
		}
	}
}
