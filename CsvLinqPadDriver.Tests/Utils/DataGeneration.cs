using System;
using System.Collections.Generic;
using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis;

namespace CsvLinqPadDriver.Tests.Utils
{
    internal static class DataGeneration
    {
        public static IEnumerable<FileModel> GetLargeSchemaModel()
        {
            var data = new DataDescription(new[]
                {
                    "Header1", "Header2", "Header3", "Header4", "Header5", "Header6", "Header7", "Header8", "Header9"
                },
                true);

            for (int i = 0; i < 1000; i++)
            {
                var file = new FileDescription($"File{i:000}.csv", 0, DateTime.MinValue);

                yield return new FileModel(file, data);
            }
        }

        public static SyntaxTree GetSyntaxTree(IEnumerable<FileModel> schemaModel)
        {
            var syntaxTreeBuilder = new SyntaxTreeBuilder("TestContextClass");
            foreach (var fileModel in schemaModel)
            {
                syntaxTreeBuilder.AddModel(fileModel);
            }

            return syntaxTreeBuilder.Build("TestNamespace");
        }

        public static IEnumerable<ExplorerItem> GetSchema(IEnumerable<FileModel> schemaModel)
        {
            var builder = new SchemaBuilder();
            foreach (var fileModel in schemaModel)
            {
                builder.AddModel(fileModel);
            }

            return builder.Build();
        }
    }
}