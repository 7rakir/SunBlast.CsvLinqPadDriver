using LINQPad.Extensibility.DataContext;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System.Windows.Forms;

namespace Tarydium.CSVLINQPadDriver
{
	public class DynamicDriver : DynamicDataContextDriver
	{
		static DynamicDriver()
		{
			AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
			{
				if(args.Exception.StackTrace.Contains("Tarydium.CSVLINQPadDriver"))
					Debugger.Launch();
			};

			CsvReaderAssemblyLocation = typeof(CsvParser.CsvReader).Assembly.Location;
			LoadAssemblySafely(CsvReaderAssemblyLocation);
		}

		public override string Name => "CSV to LINQ driver";

		public override string Author => "Drakir";

		private static string CsvReaderAssemblyLocation { get; }

		public override string GetConnectionDescription(IConnectionInfo cxInfo)
			=> "Transforming CSV to queryable objects using LINQ";

		public override bool ShowConnectionDialog(IConnectionInfo cxInfo, ConnectionDialogOptions dialogOptions)
		{
			var dialog = new FolderBrowserDialog()
			{
				Description = "Select a folder containing the desired CSV files", 
				UseDescriptionForTitle = true
			};
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				cxInfo.DisplayName = dialog.SelectedPath;
				return true;
			}
			return false;
		}

		public override List<ExplorerItem> GetSchemaAndBuildAssembly(
			IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName)
		{
			string path = cxInfo.DisplayName;
			var schema = SchemaReader.GetSchema(path).ToArray();

			AppendClassesToAssembly(assemblyToBuild, nameSpace, typeName, schema);

			return TreeGenerator.GetTree(schema);
		}

		private static void AppendClassesToAssembly(AssemblyName assemblyToBuild, string nameSpace, string typeName, IEnumerable<FileModel> schema)
		{
			var references = GetCoreFxReferenceAssemblies().Append(CsvReaderAssemblyLocation);

			var result = ClassGenerator.Generate(assemblyToBuild, nameSpace, references, typeName, schema);

			if (!result.Success)
			{
				LogError(result);
			}
		}

		private static void LogError(EmitResult result)
		{
			var message = string.Join(Environment.NewLine,
				result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()));

			WriteToLog(message, "Tarydium");
		}
	}
}