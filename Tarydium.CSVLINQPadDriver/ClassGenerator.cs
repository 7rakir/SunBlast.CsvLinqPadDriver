using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Tarydium.CSVLINQPadDriver
{
	class ClassGenerator
	{
		public void Generate(AssemblyName assembly, string nameSpace, string[] references, string className, IEnumerable<DataFile> files)
		{
			var syntaxTree = new SyntaxTreeGenerator(nameSpace, className).GetSyntaxTree(files);

			var compilation = GetCompilation(references, assembly, syntaxTree);

			Emit(assembly, compilation);
		}

		private void Emit(AssemblyName assembly, Compilation compilation)
		{
			using var fileStream = File.OpenWrite(assembly.CodeBase);

			var results = compilation.Emit(fileStream);

			if(results.Success)
			{
				return;
			}

			Debugger.Launch();

			var message = string.Join(". ",
				results.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()));

			throw new InvalidOperationException(message);
		}

		private Compilation GetCompilation(string[] references, AssemblyName assembly, SyntaxTree syntaxTree)
		{
			var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

			var refs = references.Select(x => MetadataReference.CreateFromFile(x));

			return CSharpCompilation.Create(assembly.FullName).WithOptions(options).AddReferences(refs).AddSyntaxTrees(syntaxTree);
		}
	}
}
