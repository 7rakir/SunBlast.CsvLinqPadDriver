using System;
using LINQPad.Extensibility.DataContext;
using System.Collections.Generic;
using System.Linq;

namespace CsvLinqPadDriver
{
	internal class SchemaBuilder
	{
		// collection of multiple categories while each category contains multiple file models
		private readonly SortedDictionary<string, SortedDictionary<string, FileModel>> schema =
			new(StringComparer.InvariantCultureIgnoreCase);

		public void AddModel(FileModel fileModel)
		{
			var prefix = PrefixProvider.GetPrefix(fileModel);
			if (!schema.TryGetValue(prefix, out var category))
			{
				category = new SortedDictionary<string, FileModel>();
				schema.Add(prefix, category);
			}

			category.Add(fileModel.ClassName, fileModel);
		}
		
		public IEnumerable<ExplorerItem> Build()
		{
			foreach (var (prefix, category) in schema)
			{
				int itemCount = category.Values.Count;
				if (itemCount == 1)
				{
					yield return ExplorerItemFactory.GetModelExplorerItem(category.Values.Single());
				}
				else
				{
					yield return ExplorerItemFactory.GetCategoryExplorerItem($"{prefix} ({itemCount})", category.Values);
				}
			}
		}

		private static class PrefixProvider
		{
			private static readonly string[] CustomPrefixes = { "Alert", "cbQoS", "NetFlow", "VoIP" };
			
			public static string GetPrefix(FileModel model)
			{
				return GetPrefix(model.ClassName);
			}
			
			private static string GetPrefix(string className)
			{
				if (TryGetCustomPrefix(className, out var prefix))
				{
					return prefix!;
				}

				int prefixIndex = className.IndexOf('_', StringComparison.Ordinal);

				return prefixIndex <= 0 ? className : className[..prefixIndex];
			}

			private static bool TryGetCustomPrefix(string className, out string? prefix)
			{
				prefix = CustomPrefixes.FirstOrDefault(custom => className.StartsWith(custom, StringComparison.InvariantCultureIgnoreCase));
				return prefix != null;
			}
		}

		private static class ExplorerItemFactory
		{
			public static ExplorerItem GetCategoryExplorerItem(string text, IEnumerable<FileModel> fileModels)
			{
				return new(text, ExplorerItemKind.Category, ExplorerIcon.Schema)
				{
					Children = fileModels.Select(GetModelExplorerItem).ToList()
				};
			}

			public static ExplorerItem GetModelExplorerItem(FileModel fileModel)
			{
				if (!fileModel.HasData)
				{
					return new($"{fileModel.ClassName}", ExplorerItemKind.Property, ExplorerIcon.Blank)
					{
						IsEnumerable = false,
						Children = fileModel.Headers.Select(GetPropertyExplorerItem).ToList(),
						ToolTipText = GetToolTip(fileModel)
					};
				}
			
				return new($"{fileModel.ClassName}", ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
				{
					IsEnumerable = true,
					Children = fileModel.Headers.Select(GetPropertyExplorerItem).ToList(),
					ToolTipText = GetToolTip(fileModel)
				};
			}

			private static ExplorerItem GetPropertyExplorerItem(string columnName)
			{
				return new(columnName, ExplorerItemKind.Property, ExplorerIcon.Column);
			}

			private static string GetToolTip(FileModel fileModel)
			{
				return $"Length: {fileModel.FileSize:N0} B{Environment.NewLine}Created: {fileModel.CreationTime:s}";
			}
		}
	}
}
