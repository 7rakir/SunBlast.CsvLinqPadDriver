using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using CsvLinqPadDriver.DataExtensions;

namespace CsvLinqPadDriver
{
	internal static class AssemblyHelper
	{
		private static readonly string CsvReaderAssemblyLocation = typeof(CsvParser.CsvReader).Assembly.Location;

		private static readonly string ExtensionsAssemblyLocation = typeof(DynamicExtensions).Assembly.Location;

		public static void LoadAssemblies()
		{
			DataContextDriver.LoadAssemblySafely(CsvReaderAssemblyLocation);
		}

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