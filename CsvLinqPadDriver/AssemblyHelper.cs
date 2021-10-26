using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using CsvLinqPadDriver.Csv;

namespace CsvLinqPadDriver
{
	internal static class AssemblyHelper
	{
		private static readonly string CsvReaderAssemblyLocation = typeof(CsvReader).Assembly.Location;

		public static IEnumerable<MetadataReference> GetReferences()
		{
			return DataContextDriver
				.GetCoreFxReferenceAssemblies()
				.Append(CsvReaderAssemblyLocation)
				.Select(x => MetadataReference.CreateFromFile(x));
		}
	}
}