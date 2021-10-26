using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using CsvLinqPadDriver.Extensions.Dynamic;

namespace CsvLinqPadDriver
{
	internal static class AssemblyHelper
	{
		private static readonly string CsvReaderAssemblyLocation = typeof(CsvParser.CsvReader).Assembly.Location;

		private static readonly string ExtensionsAssemblyLocation = typeof(DynamicExtensions).Assembly.Location;

		public static IEnumerable<MetadataReference> GetReferences()
		{
			return DataContextDriver
				.GetCoreFxReferenceAssemblies()
				.Append(CsvReaderAssemblyLocation)
				.Append(ExtensionsAssemblyLocation)
				.Select(x => MetadataReference.CreateFromFile(x));
		}
	}
}