using System.IO;
using System.Text.RegularExpressions;


namespace Tarydium.CSVLINQPadDriver
{
	public class FileModel
	{
		private const string InvalidCharactersRegex = @"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]";
		private static readonly Regex Regex = new Regex(InvalidCharactersRegex);

		public string ClassName { get; }

		public string FilePath { get; }

		public string[] Headers { get; }

		public FileModel(string filePath, string[] headers)
		{
			FilePath = filePath;

			Headers = headers;

			ClassName = GenerateValidIdentifierName(filePath);
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

		public class Header
		{
			public string Name { get; set; }

			public string Type { get; set; }
		}
	}
}
