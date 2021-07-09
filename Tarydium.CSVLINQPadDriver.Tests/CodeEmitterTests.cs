using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using CsvParser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Tarydium.CSVLINQPadDriver.Tests
{
    [TestFixture]
    public class CodeEmitterTests
    {
        [Test]
        public void Test5()
        {
        	var schemaModel = DataGeneration.GetLargeSchemaModel().ToArray();
        	var syntaxTree = DataGeneration.GetSyntaxTree(schemaModel);

        	var assembly = new AssemblyName("AssemblyName");
        	
        	var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        	
        	var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        	var references = new string[]{}
        		.Append(typeof(CsvReader).Assembly.Location)
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
        	
        	Assert.That(watch.ElapsedMilliseconds, Is.LessThan(1700));
        }
    }
}