using LINQPad;
using LINQPad.Extensibility.DataContext;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
		}

		public override string Name => "CSV to LINQ driver";

		public override string Author => "Drakir";

		public override string GetConnectionDescription(IConnectionInfo cxInfo)
			=> "Transforming CSV to queryable objects using LINQ";

		public override bool ShowConnectionDialog(IConnectionInfo cxInfo, ConnectionDialogOptions dialogOptions)
		{

			return new ConnectionDialog(cxInfo).ShowDialog() == true;
		}

		public override List<ExplorerItem> GetSchemaAndBuildAssembly(
			IConnectionInfo cxInfo, AssemblyName assemblyToBuild, ref string nameSpace, ref string typeName)
		{
			string path = @"c:\Users\richard.sefr\Documents\db\";

			var model = GetModel(path).ToArray();

			AppendClassesToAssembly(assemblyToBuild, nameSpace, typeName, model);

			return GetExplorerItems(model).ToList();
		}

		private ExplorerItem GetExplorerItem(DataFile file)
		{
			var item = new ExplorerItem(file.ClassName, ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
			{
				IsEnumerable = true,
				Children = file.Headers.Select(h => new ExplorerItem(h, ExplorerItemKind.Property, ExplorerIcon.Column)).ToList(),
			};

			return item;
		}

		private IEnumerable<DataFile> GetModel(string path)
		{
			var reader = new CsvReader();

			return GetFiles(path).Select(f => new DataFile(reader, f));
		}

		private void AppendClassesToAssembly(AssemblyName assemblyToBuild, string nameSpace, string typeName, IEnumerable<DataFile> model)
		{
			var generator = new ClassGenerator();

			var references = GetCoreFxReferenceAssemblies();

			generator.Generate(assemblyToBuild, nameSpace, references, typeName, model);
		}

		private IEnumerable<ExplorerItem> GetExplorerItems(IEnumerable<DataFile> model)
		{
			return model.Select(GetExplorerItem);
		}

		private IEnumerable<string> GetFiles(string path)
		{
			return Directory.EnumerateFiles(path, "*.csv");
		}
	}
}