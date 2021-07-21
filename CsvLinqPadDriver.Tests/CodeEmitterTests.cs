using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CsvLinqPadDriver.Tests
{
    [TestFixture]
    public class CodeEmitterTests
    {
        [Test]
        public void CodeEmittingShouldBeReasonablySlow()
        {
        	var schemaModel = DataGeneration.GetLargeSchemaModel().ToArray();
        	var syntaxTree = DataGeneration.GetSyntaxTree(schemaModel);

        	var assembly = new AssemblyName("AssemblyName");
        	
        	var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        	
        	var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            
            var references = new string[]{}
        		.Append(AssemblyHelper.CsvReaderAssemblyLocation)
        		.Append(typeof(object).Assembly.Location)
        		.Append(typeof(Enumerable).Assembly.Location)
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
        	
        	Assert.That(watch.ElapsedMilliseconds, Is.LessThan(1750));
        }
    }
}