using System.Collections.Generic;
using Il2CppSystem;
using Unity.Entities;

namespace KindredExtract.Models;

public class EcsSystemTreeNode
{
	public EcsSystemCategory Category;

	public Type Type;

	public SystemHandle SystemHandle;

	public ComponentSystemBase Instance;

	public IList<EcsSystemTreeNode> ChildrenOrderedForUpdate;

	public IList<EcsSystemTreeNode> Parents;

	public EcsSystemTreeNode(EcsSystemCategory category, SystemHandle systemHandle, Type type = null, ComponentSystemBase instance = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Category = category;
		Type = type;
		SystemHandle = systemHandle;
		Instance = instance;
		ChildrenOrderedForUpdate = new List<EcsSystemTreeNode>();
		Parents = new List<EcsSystemTreeNode>();
		base._002Ector();
	}

	public int CountDescendants()
	{
		int num = 0;
		foreach (EcsSystemTreeNode item in ChildrenOrderedForUpdate)
		{
			num++;
			num += item.CountDescendants();
		}
		return num;
	}
}
