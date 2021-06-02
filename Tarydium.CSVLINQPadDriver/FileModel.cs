using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Tarydium.CSVLINQPadDriver
{
	public class FileModel
	{
		private const string InvalidCharactersRegex = @"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]";
		private static readonly Regex Regex = new(InvalidCharactersRegex);

		public string Prefix { get; }

		public string ClassName { get; }

		public string FilePath { get; }

		public string[] Headers { get; }

		public FileModel(string filePath, string[] headers)
		{
			FilePath = filePath;

			Headers = headers;

			(Prefix, ClassName) = GenerateValidIdentifierName(filePath);
		}

		private static (string, string) GenerateValidIdentifierName(string filePath)
		{
			var result = Path.GetFileNameWithoutExtension(filePath);

			result = Regex.Replace(result, "_");

			if(!char.IsLetter(result, 0))
			{
				result = result.Insert(0, "_");
			}

			result = result.Replace(" ", string.Empty);

			int underscoreIndex = result.IndexOf("_", StringComparison.Ordinal);

			if (underscoreIndex == -1)
			{
				return (null, result);
			}

			var prefix = result[..underscoreIndex];

			return (prefix, result);
		}
	}
}
