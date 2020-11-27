using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tarydium.CSVLINQPadDriver
{
	public class DataFile
	{
		private readonly IDataReader dataReader;
		private readonly string path;

		public string[] Headers => dataReader.GetHeaders(path);

		public IEnumerable<string[]> Data => dataReader.GetData(path);

		public string ClassName => Path.GetFileNameWithoutExtension(path);

		public DataFile(IDataReader dataReader, string path)
		{
			this.dataReader = dataReader;
			this.path = path;
		}
	}
}
