using System;
using System.Diagnostics;
using NUnit.Framework;

namespace CsvLinqPadDriver.Tests.Utils
{
	internal class DurationAssert : IDisposable
	{
		private readonly int expectedMaximumDuration;
		private readonly Stopwatch watch;

		private DurationAssert(int expectedMaximumDuration)
		{
			this.expectedMaximumDuration = expectedMaximumDuration;
			watch = Stopwatch.StartNew();
		}

		public static DurationAssert StartNew(int expectedMaximumDuration) => new(expectedMaximumDuration);

		public void Dispose()
		{
			watch.Stop();

			Console.WriteLine(watch.Elapsed);

			Assert.That(watch.ElapsedMilliseconds, Is.LessThan(expectedMaximumDuration));
		}
	}
}
