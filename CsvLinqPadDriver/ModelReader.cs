using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CsvLinqPadDriver
{
	internal static class ModelReader
	{
		public static async IAsyncEnumerable<FileModel> GetSchemaModelAsync(string path)
		{
			var files = GetFiles(path);
			foreach (var file in files)
			{
				var model = await GetModelAsync(file);
				if (model != null)
				{
					yield return model;
				}
			}
		}

		private static async Task<FileModel?> GetModelAsync(FileDescription file)
		{
			var data = await GetDataAsync(file.FullPath);

			return data is null ? null : new FileModel(file, data);
		}

		private static async Task<DataDescription?> GetDataAsync(string filePath)
		{
			await using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			using var reader = new StreamReader(stream);
			
			var headerLine = await reader.ReadLineAsync();

			if (string.IsNullOrWhiteSpace(headerLine))
			{
				return null;
			}
			
			var headers = headerLine.Split(',', StringSplitOptions.RemoveEmptyEntries);
			
			var dataLine = await reader.ReadLineAsync();

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
