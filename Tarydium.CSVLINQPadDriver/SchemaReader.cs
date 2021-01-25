using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tarydium.CSVLINQPadDriver
{
	public interface ISchemaReader
	{
		IEnumerable<FileModel> GetSchema(string path);
	}

	internal class SchemaReader : ISchemaReader
	{
		public IEnumerable<FileModel> GetSchema(string path)
		{
			return GetFiles(path).Select(GetModel).Where(m => m != null);
		}

		private static FileModel GetModel(string filePath)
		{
			var firstLine = GetFirstLine(filePath);

			if (string.IsNullOrWhiteSpace(firstLine))
			{
				return null;
			}

			var headers = firstLine.Split(",", StringSplitOptions.RemoveEmptyEntries);
			return new FileModel(filePath, headers);
		}

		private static string GetFirstLine(string filePath)
		{
			using var stream = File.OpenRead(filePath);

			using var reader = new StreamReader(stream);

			return reader.ReadLine();
		}

		private static IEnumerable<string> GetFiles(string path)
		{
			return Directory.EnumerateFiles(path, "*.csv");
		}
	}
}
