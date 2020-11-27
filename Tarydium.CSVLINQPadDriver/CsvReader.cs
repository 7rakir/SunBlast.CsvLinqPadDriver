using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tarydium.CSVLINQPadDriver
{
	public interface IDataReader
	{
		string[] GetHeaders(string path);

		IEnumerable<string[]> GetData(string path);
	}

	class CsvReader : IDataReader
	{
		public string[] GetHeaders(string path)
		{
			return File.ReadLines(path).First().Split(",");
		}

		public IEnumerable<string[]> GetData(string path)
		{
			return File.ReadLines(path).Skip(1).Select(l => l.Split(","));
		}
	}
}
