﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Tarydium.CSVLINQPadDriver
{
	internal static class ClassGenerator
	{
		public static EmitResult Generate(AssemblyName assembly, string nameSpace, IEnumerable<string> references, string className, IEnumerable<FileModel> schema)
		{
			var syntaxTree = SyntaxTreeGenerator.GetSyntaxTree(nameSpace, className, schema);

			var compilation = GetCompilation(references, assembly, syntaxTree);

			return Emit(assembly, compilation);
		}

		private static EmitResult Emit(AssemblyName assembly, Compilation compilation)
		{
			using var fileStream = File.OpenWrite(assembly.CodeBase);

			return compilation.Emit(fileStream);
		}

		private static Compilation GetCompilation(IEnumerable<string> references, AssemblyName assembly, SyntaxTree syntaxTree)
		{
			var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

			var refs = references.Select(x => MetadataReference.CreateFromFile(x));

			return CSharpCompilation.Create(assembly.FullName).WithOptions(options).AddReferences(refs).AddSyntaxTrees(syntaxTree);
		}
	}
}
