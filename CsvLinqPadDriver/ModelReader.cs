using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvLinqPadDriver
{
	internal static class ModelReader
	{
		public static IEnumerable<FileModel> GetSchemaModel(string path)
		{
			var files = GetFiles(path);
			foreach (var file in files)
			{
				var model = GetModel(file);
				if (model != null)
				{
					yield return model;
				}
			}
		}

		private static FileModel? GetModel(FileDescription file)
		{
			var data = GetData(file.FullPath);

			return data is null ? null : new FileModel(file, data);
		}

		private static DataDescription? GetData(string filePath)
		{
			using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			using var reader = new StreamReader(stream);
			
			var headerLine = reader.ReadLine();

			if (string.IsNullOrWhiteSpace(headerLine))
			{
				return null;
			}
			
			var headers = headerLine.Split(',', StringSplitOptions.RemoveEmptyEntries);
			
			var dataLine = reader.ReadLine();

			var hasData = !string.IsNullOrWhiteSpace(dataLine);

			return new DataDescription(headers, hasData);
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
