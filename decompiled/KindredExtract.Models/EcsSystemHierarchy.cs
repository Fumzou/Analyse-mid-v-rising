using System.Collections.Generic;
using Unity.Entities;

namespace KindredExtract.Models;

public class EcsSystemHierarchy
{
	public World World;

	public EcsSystemCounts Counts;

	public KnownUnknowns KnownUnknowns;

	public IList<EcsSystemTreeNode> RootNodesUnordered;

	public IList<EcsSystemTreeNode> FindNodesWithMultipleParents()
	{
		List<EcsSystemTreeNode> list = new List<EcsSystemTreeNode>();
		foreach (EcsSystemTreeNode item in RootNodesUnordered)
		{
			FindNodesWithMultipleParents(item, list);
		}
		return list;
	}

	private void FindNodesWithMultipleParents(EcsSystemTreeNode node, IList<EcsSystemTreeNode> foundNodes)
	{
		if (node.Parents.Count > 1)
		{
			foundNodes.Add(node);
		}
		foreach (EcsSystemTreeNode item in node.ChildrenOrderedForUpdate)
		{
			FindNodesWithMultipleParents(item, foundNodes);
		}
	}
}
