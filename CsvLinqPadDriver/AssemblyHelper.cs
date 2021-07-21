using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace CsvLinqPadDriver
{
	internal static class AssemblyHelper
	{
		public static readonly string CsvReaderAssemblyLocation = typeof(CsvParser.CsvReader).Assembly.Location;

		public static void LoadAssemblies()
		{
			DataContextDriver.LoadAssemblySafely(CsvReaderAssemblyLocation);
		}

		public static IEnumerable<MetadataReference> GetReferences()
		{
			return DataContextDriver
				.GetCoreFxReferenceAssemblies()
				.Append(CsvReaderAssemblyLocation)
				.Select(x => MetadataReference.CreateFromFile(x));
		}
	}
}