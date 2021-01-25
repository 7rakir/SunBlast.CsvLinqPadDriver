using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CsvParser
{
	public static class CsvReader
	{
		public static IEnumerable<T> ReadFile<T>(string path)
		{
			using var reader = new StreamReader(path);
			using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
			return csv.GetRecords<T>().ToArray();
		}
	}
}
