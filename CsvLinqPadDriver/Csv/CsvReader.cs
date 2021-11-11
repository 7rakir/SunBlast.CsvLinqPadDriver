using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper.Configuration;

namespace CsvLinqPadDriver.Csv
{
    public static class CsvReader
    {
        private static readonly CsvConfiguration Configuration = new(CultureInfo.InvariantCulture)
        {
            MemberTypes = MemberTypes.Fields
        };

        public static IEnumerable<T> ReadFile<T>(string path)
        {
            var reader = new StreamReader(path);
            var csv = new CsvHelper.CsvReader(reader, Configuration);
            try
            {
                foreach (var record in csv.GetRecords<T>())
                {
                    yield return record;
                }
            }
            finally
            {
                reader.Dispose();
                csv.Dispose();
            }
        }
    }
}