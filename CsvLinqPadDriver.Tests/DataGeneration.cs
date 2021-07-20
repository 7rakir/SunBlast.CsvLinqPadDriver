using System.Collections.Generic;
using LINQPad.Extensibility.DataContext;
using Microsoft.CodeAnalysis;

namespace CsvLinqPadDriver.Tests
{
    internal static class DataGeneration
    {
        public static IEnumerable<FileModel> GetLargeSchemaModel()
        {
            for (int i = 0; i < 1000; i++)
            {
                yield return new FileModel($"File{i:000}.csv",
                    new[]
                    {
                        "Header1", "Header2", "Header3", "Header4", "Header5", "Header6", "Header7", "Header8", "Header9"
                    });
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
            return builder.BuildSchema();
        }
    }
}