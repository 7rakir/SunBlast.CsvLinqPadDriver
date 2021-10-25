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
			var customPrefixes = new[] { "alert", "cbqos", "netflow", "voip" };

			foreach (var customPrefix in customPrefixes)
			{
				if (className.StartsWith(customPrefix, StringComparison.InvariantCultureIgnoreCase))
				{
					return className[..customPrefix.Length];
				}
			}
			
			int prefixIndex = className.IndexOf('_', StringComparison.Ordinal);

			if (prefixIndex == -1)
			{
				return null;
			}

			return className[..prefixIndex];
		}
	}
}
