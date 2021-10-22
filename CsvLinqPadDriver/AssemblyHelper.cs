using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using CsvLinqPadDriver.DataExtensions;

namespace CsvLinqPadDriver
{
	internal static class AssemblyHelper
	{
		public static readonly string CsvReaderAssemblyLocation = typeof(CsvParser.CsvReader).Assembly.Location;
		
		public static readonly string ExtensionsAssemblyLocation = typeof(DynamicExtensions).Assembly.Location;

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