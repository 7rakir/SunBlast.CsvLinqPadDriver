using System;
using System.IO;
using System.Linq;
using System.Reflection;
using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Tarydium.CSVLINQPadDriver
{
	public static class CodeEmitter
	{
		static CodeEmitter()
		{
			CsvReaderAssemblyLocation = typeof(CsvParser.CsvReader).Assembly.Location;
			DataContextDriver.LoadAssemblySafely(CsvReaderAssemblyLocation);
		}

		private static string CsvReaderAssemblyLocation { get; }

		public static void Emit(SyntaxTree syntaxTree, AssemblyName assembly)
		{
			var compilation = GetCompilation(assembly, syntaxTree);
			
			using var fileStream = File.OpenWrite(assembly.CodeBase!);
			
			var result = compilation.Emit(fileStream);
			
			if(!result.Success)
			{
				LogError(result);
			}
		}

		private static Compilation GetCompilation(AssemblyName assembly, SyntaxTree syntaxTree)
		{
			var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

			var references = DataContextDriver
				.GetCoreFxReferenceAssemblies()
				.Append(CsvReaderAssemblyLocation)
				.Select(x => MetadataReference.CreateFromFile(x));

			return CSharpCompilation
				.Create(assembly.FullName)
				.WithOptions(options)
				.AddReferences(references)
				.AddSyntaxTrees(syntaxTree);
		}
		
		private static void LogError(EmitResult result)
		{
			var message = string.Join(Environment.NewLine,
				result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()));

			DynamicDriver.WriteToLog(message);
		}
	}
}
