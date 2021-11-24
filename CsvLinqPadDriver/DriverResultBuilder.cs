using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LINQPad.Extensibility.DataContext;

namespace CsvLinqPadDriver
{
    internal class DriverResultBuilder
    {
        private readonly string nameSpace;
        private readonly SyntaxTreeBuilder syntaxTreeBuilder;
        private readonly SchemaBuilder schemaBuilder;

        public DriverResultBuilder(ref string nameSpace, ref string typeName)
        {
            this.nameSpace = nameSpace;
            syntaxTreeBuilder = new SyntaxTreeBuilder(typeName);
            schemaBuilder = new SchemaBuilder();
        }

        public DriverResultBuilder ApplyModelFrom(string csvFilesPath)
        {
            var schemaModel = ModelReader.GetSchemaModel(csvFilesPath);
            foreach (var fileModel in schemaModel)
            {
                syntaxTreeBuilder.AddModel(fileModel);
                schemaBuilder.AddModel(fileModel);
            }
            return this;
        }

        public DriverResultBuilder EmitInto(AssemblyName assembly)
        {
            var tree = syntaxTreeBuilder.Build(nameSpace);
            CodeEmitter.Emit(tree, assembly);
            return this;
        }

        public List<ExplorerItem> BuildSchema()
        {
            return schemaBuilder.Build().ToList();
        }
    }
}