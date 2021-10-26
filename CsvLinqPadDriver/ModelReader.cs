using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace CsvLinqPadDriver
{
	internal static class ModelReader
	{
		public static IEnumerable<FileModel> GetSchemaModel(string path)
		{
			var files = GetFiles(path);
			foreach (var file in files)
			{
				var data = GetData(file.FullPath);
				if (data != null)
				{
					yield return new FileModel(file, data);
				}
			}
		}

		private static DataDescription? GetData(string filePath)
		{
			using var reader = new StreamReader(filePath);

			using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

			var hasHeader = csv.Read();
			if (!hasHeader)
			{
				return null;
			}
			
			csv.ReadHeader();
			var hasData = csv.Read();

			return new DataDescription(csv.HeaderRecord, hasData);
		}

		private static IEnumerable<FileDescription> GetFiles(string path)
		{
			return new DirectoryInfo(path)
				.EnumerateFiles("*.csv", SearchOption.AllDirectories)
				.Select(f => new FileDescription(f.FullName, f.Length, f.CreationTimeUtc));
		}
	}

	internal record FileDescription(string FullPath, long Length, DateTime CreationTime);

	internal record DataDescription(string[] Headers, bool HasData);
}
