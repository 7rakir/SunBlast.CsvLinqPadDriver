using NUnit.Framework;
using System;
using System.Diagnostics;

namespace CsvLinqPadDriver.Tests
{
	internal class DurationAssert : IDisposable
	{
		private readonly int _expectedMaximumDuration;
		private readonly Stopwatch _watch;

		private DurationAssert(int expectedMaximumDuration)
		{
			_expectedMaximumDuration = expectedMaximumDuration;
			_watch = Stopwatch.StartNew();
		}

		public static DurationAssert StartNew(int expectedMaximumDuration) => new(expectedMaximumDuration);

		public void Dispose()
		{
			_watch.Stop();

			Console.WriteLine(_watch.Elapsed);

			Assert.That(_watch.ElapsedMilliseconds, Is.LessThan(_expectedMaximumDuration));
		}
	}
}
