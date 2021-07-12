using LINQPad.Extensibility.DataContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tarydium.CSVLINQPadDriver
{
	public class DynamicDriver : DynamicDataContextDriver
	{
		static DynamicDriver()
		{
			AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
			{
				if(args.Exception.StackTrace?.Contains("Tarydium.CSVLINQPadDriver") is true)
					Debugger.Launch();
			};
		}

		public override string Name => "CSV to LINQ driver";

		public override string Author => "Drakir";

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

			var syntaxTreeBuilder = new SyntaxTreeBuilder(typeName);
			var schemaBuilder = new SchemaBuilder();
			
			ApplyModelAsync(path, syntaxTreeBuilder, schemaBuilder).Wait();

			var tree = syntaxTreeBuilder.Build(nameSpace);
			CodeEmitter.Emit(tree, assemblyToBuild);
			
			return schemaBuilder.BuildSchema().ToList();
		}

		private static async Task ApplyModelAsync(string path, SyntaxTreeBuilder syntaxTreeBuilder, SchemaBuilder schemaBuilder)
		{
			await foreach (var fileModel in ModelReader.GetSchemaModelAsync(path))
			{
				syntaxTreeBuilder.AddModel(fileModel);
				schemaBuilder.AddModel(fileModel);
			}
		}

		public static void WriteToLog(string message) => WriteToLog(message, "Tarydium");
	}
}