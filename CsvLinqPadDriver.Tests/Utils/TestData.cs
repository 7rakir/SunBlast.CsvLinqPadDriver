using System;

namespace CsvLinqPadDriver.Tests.Utils
{
    public static class TestData
    {
        public static string Path<T>()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", typeof(T).Name);
        }
        
        public static string Path<T>(string fileName) => System.IO.Path.Combine(Path<T>(), fileName);
    }
}
