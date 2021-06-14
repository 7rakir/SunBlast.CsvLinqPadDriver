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

			var classGenerator = new ClassGenerator(typeName);
			var treeGenerator = new TreeGenerator();
			
			foreach (var fileModel in SchemaReader.GetSchema(path))
			{
				classGenerator.Add(fileModel);
				treeGenerator.Add(fileModel);
			}
			
			var references = GetCoreFxReferenceAssemblies().Append(CsvReaderAssemblyLocation);
			var result = classGenerator.Build(assemblyToBuild, nameSpace, references);
			
			if (!result.Success)
			{
				LogError(result);
			}

			return treeGenerator.Build().ToList();
		}

		private static void LogError(EmitResult result)
		{
			var message = string.Join(Environment.NewLine,
				result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()));

			WriteToLog(message, "Tarydium");
		}
	}
}