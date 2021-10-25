using System;
using LINQPad.Extensibility.DataContext;
using System.Collections.Generic;
using System.Linq;

namespace CsvLinqPadDriver
{
	internal class SchemaBuilder
	{
		private readonly SortedDictionary<string, SortedDictionary<string, FileModel>> schema =
			new(StringComparer.InvariantCultureIgnoreCase);

		public void AddModel(FileModel fileModel)
		{
			var prefix = fileModel.Prefix ?? fileModel.ClassName;
			if (!schema.TryGetValue(prefix, out var category))
			{
				category = new SortedDictionary<string, FileModel>();
				schema.Add(prefix, category);
			}

			category.Add(fileModel.ClassName, fileModel);
		}
		
		public IEnumerable<ExplorerItem> BuildSchema()
		{
			foreach (var (prefix, category) in schema)
			{
				int categoryCount = category.Values.Count;
				if (categoryCount == 1)
				{
					yield return GetModelExplorerItem(category.Values.Single());
				}
				else
				{
					yield return GetCategoryExplorerItem($"{prefix} ({categoryCount})", category.Values);
				}
			}
		}

		private static ExplorerItem GetCategoryExplorerItem(string text, IEnumerable<FileModel> fileModels)
		{
			return new(text, ExplorerItemKind.Category, ExplorerIcon.Schema)
			{
				Children = fileModels.Select(GetModelExplorerItem).ToList()
			};
		}

		private static ExplorerItem GetModelExplorerItem(FileModel fileModel)
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
