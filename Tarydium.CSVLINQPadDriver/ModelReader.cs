using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tarydium.CSVLINQPadDriver
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

		public static IEnumerable<FileModel> GetSchemaModel(string path)
		{
			return GetFiles(path).Select(GetModel).Where(m => m != null)!;
		}

		private static async Task<FileModel?> GetModelAsync(string filePath)
		{
			var firstLine = await GetFirstLineAsync(filePath);

			if (string.IsNullOrWhiteSpace(firstLine))
			{
				return null;
			}

			var headers = firstLine.Split(",", StringSplitOptions.RemoveEmptyEntries);
			return new FileModel(filePath, headers);
		}

		private static FileModel? GetModel(string filePath)
		{
			var firstLine = GetFirstLine(filePath);

			if(string.IsNullOrWhiteSpace(firstLine))
			{
				return null;
			}

			var headers = firstLine.Split(",", StringSplitOptions.RemoveEmptyEntries);
			return new FileModel(filePath, headers);
		}

		private static async Task<string?> GetFirstLineAsync(string filePath)
		{
			await using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			using var reader = new StreamReader(stream);

			return await reader.ReadLineAsync();
		}

		private static string? GetFirstLine(string filePath)
		{
			using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			using var reader = new StreamReader(stream);

			return reader.ReadLine();
		}

		private static IEnumerable<string> GetFiles(string path)
		{
			return Directory.EnumerateFiles(path, "*.csv");
		}
	}
}
