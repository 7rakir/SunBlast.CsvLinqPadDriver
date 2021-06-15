using System;
using System.Diagnostics;

namespace Tarydium.CSVLINQPadDriver
{
	internal class DurationLogger : IDisposable
	{
		private readonly string _name;
		private readonly Stopwatch _watch;

		public DurationLogger(string name)
		{
			_name = name;
			_watch = Stopwatch.StartNew();
		}

		public void Dispose()
		{
			_watch.Stop();

			DynamicDriver.WriteToLog($"{_name}: { _watch.ElapsedMilliseconds} ms");
		}
	}
}
