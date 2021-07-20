using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CsvLinqPadDriver
{
	internal class FileModel
	{
		private const string InvalidCharactersRegex = @"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]";
		private static readonly Regex Regex = new(InvalidCharactersRegex);

		public string? Prefix { get; }

		public string ClassName { get; }

		public string FilePath { get; }

		public string[] Headers { get; }

		public FileModel(string filePath, string[] headers)
		{
			FilePath = filePath;

			Headers = headers;

			ClassName = GenerateValidIdentifierName(filePath);

			Prefix = GetPrefix(ClassName);
		}

		private static string GenerateValidIdentifierName(string filePath)
		{
			var result = Path.GetFileNameWithoutExtension(filePath);

			result = Regex.Replace(result, "_");

			if(!char.IsLetter(result, 0))
			{
				result = result.Insert(0, "_");
			}

			return result.Replace(" ", string.Empty);
		}

		private static string? GetPrefix(string className)
		{
			int underscoreIndex = className.IndexOf('_', StringComparison.Ordinal);

			return underscoreIndex == -1 ? null : className[..underscoreIndex];
		}
	}
}
