using System.Collections.Generic;
using System.Linq;
using LINQPad.Extensibility.DataContext;

namespace Tarydium.CSVLINQPadDriver
{
	public static class TreeGenerator
	{
		public static List<ExplorerItem> GetTree(IEnumerable<FileModel> schema)
		{
			var tree = CreateTreeStructure(schema);
			return GetExplorerItems(tree).ToList();
		}

		private static SortedDictionary<string, SortedDictionary<string, FileModel>> CreateTreeStructure(IEnumerable<FileModel> schema)
		{
			var tree = new SortedDictionary<string, SortedDictionary<string, FileModel>>();

			foreach(var model in schema)
			{
				var prefix = model.Prefix ?? model.ClassName;
				if(!tree.TryGetValue(prefix, out var category))
				{
					category = new SortedDictionary<string, FileModel>();
					tree.Add(prefix, category);
				}

				category.Add(model.ClassName, model);
			}

			return tree;
		}

		private static IEnumerable<ExplorerItem> GetExplorerItems(SortedDictionary<string, SortedDictionary<string, FileModel>> structure)
		{
			foreach(var (prefix, category) in structure)
			{
				int categoryCount = category.Values.Count;
				if(categoryCount == 1)
				{
					yield return GetModelExplorerItem(category.Values.Single());
				}
				else
				{
					yield return GetCategoryExplorerItem($"{prefix} ({categoryCount})", category.Values.Select(GetModelExplorerItem).ToList());
				}
			}
		}

		private static ExplorerItem GetCategoryExplorerItem(string prefix, List<ExplorerItem> models)
		{
			return new(prefix, ExplorerItemKind.Category, ExplorerIcon.Schema)
			{
				Children = models,
			};
		}

		private static ExplorerItem GetModelExplorerItem(FileModel fileModel)
		{
			return new(fileModel.ClassName, ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
			{
				IsEnumerable = true,
				Children = fileModel.Headers.Select(GetPropertyExplorerItem).ToList(),
			};
		}

		private static ExplorerItem GetPropertyExplorerItem(string columnName)
		{
			return new(columnName, ExplorerItemKind.Property, ExplorerIcon.Column);
		}
	}
}
