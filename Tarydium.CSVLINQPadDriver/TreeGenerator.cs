using System.Collections.Generic;
using System.Linq;
using LINQPad.Extensibility.DataContext;

namespace Tarydium.CSVLINQPadDriver
{
	public class TreeGenerator
	{
		private readonly SortedDictionary<string, SortedDictionary<string, FileModel>> tree = new();

		public void Add(FileModel fileModel)
		{
			var prefix = fileModel.Prefix ?? fileModel.ClassName;
			if(!tree.TryGetValue(prefix, out var category))
			{
				category = new SortedDictionary<string, FileModel>();
				tree.Add(prefix, category);
			}

			category.Add(fileModel.ClassName, fileModel);
		}

		public IEnumerable<ExplorerItem> Build()
		{
			return GetExplorerItems().ToList();
		}
		
		private IEnumerable<ExplorerItem> GetExplorerItems()
		{
			foreach(var (prefix, category) in tree)
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
