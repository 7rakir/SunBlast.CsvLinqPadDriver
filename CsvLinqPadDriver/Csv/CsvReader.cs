﻿using System.Collections.Generic;
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
            using var reader = GetReader(path);
            foreach (var record in reader.GetRecords<T>())
            {
                yield return record;
            }
        }

        internal static DataDescription? ReadDataDescription(string path)
        {
            using var reader = GetReader(path);

            var hasHeader = reader.Read();
            if (!hasHeader)
            {
                return null;
            }
			
            reader.ReadHeader();
            var hasData = reader.Read();

            return new DataDescription(reader.HeaderRecord, hasData);
        }

        private static CsvHelper.IReader GetReader(string path)
        {
            var reader = new StreamReader(path);
            return new CsvHelper.CsvReader(reader, Configuration);
        }
    }
}