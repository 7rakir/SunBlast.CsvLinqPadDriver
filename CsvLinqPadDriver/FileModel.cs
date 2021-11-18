using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CsvLinqPadDriver
{
	internal class FileModel
	{
		private const string InvalidCharactersRegex = @"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]";
		private static readonly Regex Regex = new(InvalidCharactersRegex);
		private static readonly string[] CustomPrefixes = { "alert", "cbqos", "netflow", "voip" };

		public string? Prefix { get; }

		public string ClassName { get; }

		public string FilePath { get; }

		public long FileSize { get; }

		public DateTime CreationTime { get; }

		public string[] Headers { get; }
		
		public bool HasData { get; }

		public FileModel(FileDescription file, DataDescription data)
		{
			(FilePath, FileSize, CreationTime) = file;

			(Headers, HasData) = data;

			ClassName = GenerateValidIdentifierName(FilePath);

			Prefix = GetPrefix(ClassName);
		}

		private static string GenerateValidIdentifierName(string filePath)
		{
			var result = Path.GetFileNameWithoutExtension(filePath);

			result = Regex.Replace(result, "_");

			if (!char.IsLetter(result, 0))
			{
				result = result.Insert(0, "_");
			}

			return result.Replace(" ", string.Empty);
		}

		private static string? GetPrefix(string className)
		{
			if (TryGetCustomPrefix(className, out var prefix))
			{
				return prefix;
			}

			int prefixIndex = className.IndexOf('_', StringComparison.Ordinal);

			return prefixIndex == -1 ? null : className[..prefixIndex];
		}

		private static bool TryGetCustomPrefix(string className, out string? prefix)
		{
			prefix = CustomPrefixes.FirstOrDefault(custom => className.StartsWith(custom, StringComparison.InvariantCultureIgnoreCase));
			return prefix != null;
		}
	}
}
