using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvLinqPadDriver.Csv;

namespace CsvLinqPadDriver
{
	internal static class ModelReader
	{
		public static IEnumerable<FileModel> GetSchemaModel(string path)
		{
			var files = GetFiles(path);
			foreach (var file in files)
			{
				var data = CsvReader.ReadDataDescription(file.FullPath);
				if (data != null)
				{
					yield return new FileModel(file, data);
				}
			}
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
