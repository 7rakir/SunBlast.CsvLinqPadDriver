using LINQPad.Extensibility.DataContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace CsvLinqPadDriver
{
	internal class DynamicDriver : DynamicDataContextDriver
	{
#if DEBUG
		static DynamicDriver()
		{
			AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
			{
				if(args.Exception.StackTrace?.Contains("CsvLinqPadDriver") is true)
					Debugger.Launch();
			};
		}
#endif

		public override string Name => "CSV to LINQ driver";

		public override string Author => "Drakir";

		public override string GetConnectionDescription(IConnectionInfo cxInfo)
			=> "Transforming CSV to queryable objects using LINQ";

		public override bool ShowConnectionDialog(IConnectionInfo cxInfo, ConnectionDialogOptions dialogOptions)
		{
			var dialog = new FolderBrowserDialog
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

			var syntaxTreeBuilder = new SyntaxTreeBuilder(typeName);
			var schemaBuilder = new SchemaBuilder();
			
			foreach (var fileModel in ModelReader.GetSchemaModel(path))
			{
				syntaxTreeBuilder.AddModel(fileModel);
				schemaBuilder.AddModel(fileModel);
			}

			var tree = syntaxTreeBuilder.Build(nameSpace);
			CodeEmitter.Emit(tree, assemblyToBuild);

		}

		public static void WriteToLog(string message) => WriteToLog(message, "SunBlast.CsvLinqPadDriver.log");
            return schemaBuilder.Build().ToList();

	}
        public override IEnumerable<string> GetNamespacesToAdd(IConnectionInfo cxInfo) => new[] { "CsvLinqPadDriver.UserExtensions.Static" };
}