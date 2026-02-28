using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using KindredExtract.Models;
using Unity.Collections;
using Unity.Entities;

namespace KindredExtract.Services;

public class EcsSystemHierarchyService
{
	private ManualLogSource Log;

	public EcsSystemHierarchyService(ManualLogSource log)
	{
		Log = log;
	}

	public EcsSystemHierarchy BuildSystemHiearchyForWorld(World world)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		ManualLogSource log = Log;
		bool flag = default(bool);
		BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(30, 1, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("building hierarchy for world: ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(world.Name);
		}
		log.LogInfo(val);
		KnownUnknowns knownUnknowns = new KnownUnknowns();
		EcsSystemCounts counts;
		Dictionary<SystemHandle, EcsSystemTreeNode> dictionary = FindSystems(world, out counts);
		foreach (EcsSystemTreeNode item in dictionary.Values.Where((EcsSystemTreeNode node) => node.Category == EcsSystemCategory.Group))
		{
			try
			{
				Enumerator<SystemHandle> enumerator2 = ((Il2CppObjectBase)item.Instance).Cast<ComponentSystemGroup>().GetAllSystems((Allocator)2).GetEnumerator();
				while (enumerator2.MoveNext())
				{
					SystemHandle current2 = enumerator2.Current;
					if (!dictionary.TryGetValue(current2, out var value))
					{
						if (TryGetSystemTypeIndex_ForMissedSystem(world, current2, out var systemTypeIndex))
						{
							value = BuildNodeAndIncrementAppropriateCount(world, systemTypeIndex, counts);
						}
						else
						{
							ManualLogSource log2 = Log;
							BepInExWarningLogInterpolatedStringHandler val2 = new BepInExWarningLogInterpolatedStringHandler(66, 2, ref flag);
							if (flag)
							{
								((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("A Group's child system does not exist within the world. Group: ");
								((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>(item.Type.FullName);
								((BepInExLogInterpolatedStringHandler)val2).AppendLiteral(" (");
								((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<EcsSystemCategory>(item.Category);
								((BepInExLogInterpolatedStringHandler)val2).AppendLiteral(")");
							}
							log2.LogWarning(val2);
							counts.Unknown++;
							knownUnknowns.SystemNotFoundInWorld.Add(current2);
							value = new EcsSystemTreeNode(EcsSystemCategory.Unknown, current2);
						}
					}
					item.ChildrenOrderedForUpdate.Add(value);
					if (value.Parents.Count > 0)
					{
						ManualLogSource log3 = Log;
						BepInExWarningLogInterpolatedStringHandler val2 = new BepInExWarningLogInterpolatedStringHandler(68, 1, ref flag);
						if (flag)
						{
							((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("Uh oh, a system belongs to multiple groups. This should not happen: ");
							((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<Type>(value.Type);
						}
						log3.LogWarning(val2);
					}
					value.Parents.Add(item);
				}
			}
			catch (Exception ex)
			{
				ManualLogSource log4 = Log;
				BepInExWarningLogInterpolatedStringHandler val2 = new BepInExWarningLogInterpolatedStringHandler(3, 2, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>(item.Type.FullName);
					((BepInExLogInterpolatedStringHandler)val2).AppendLiteral(" (");
					((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<EcsSystemCategory>(item.Category);
					((BepInExLogInterpolatedStringHandler)val2).AppendLiteral(")");
				}
				log4.LogWarning(val2);
				Log.LogWarning((object)ex);
			}
		}
		IEnumerable<EcsSystemTreeNode> source = dictionary.Values.Where((EcsSystemTreeNode node) => node.Parents.Count == 0);
		return new EcsSystemHierarchy
		{
			World = world,
			Counts = counts,
			KnownUnknowns = knownUnknowns,
			RootNodesUnordered = source.ToList()
		};
	}

	private Dictionary<SystemHandle, EcsSystemTreeNode> FindSystems(World world, out EcsSystemCounts counts)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<SystemHandle, EcsSystemTreeNode> dictionary = new Dictionary<SystemHandle, EcsSystemTreeNode>();
		counts = new EcsSystemCounts();
		Enumerator<SystemTypeIndex> enumerator = TypeManager.GetSystemTypeIndices((WorldSystemFilterFlags)(-1), (WorldSystemFilterFlags)0).GetEnumerator();
		while (enumerator.MoveNext())
		{
			SystemTypeIndex current = enumerator.Current;
			SystemHandle existingSystem = world.GetExistingSystem(current);
			WorldUnmanaged unmanaged = world.Unmanaged;
			if (!((WorldUnmanaged)(ref unmanaged)).IsSystemValid(existingSystem))
			{
				counts.NotUsed++;
				continue;
			}
			EcsSystemTreeNode value = BuildNodeAndIncrementAppropriateCount(world, current, counts);
			dictionary.Add(existingSystem, value);
		}
		return dictionary;
	}

	private EcsSystemTreeNode BuildNodeAndIncrementAppropriateCount(World world, SystemTypeIndex systemTypeIndex, EcsSystemCounts counts)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		SystemHandle existingSystem = world.GetExistingSystem(systemTypeIndex);
		WorldUnmanaged unmanaged = world.Unmanaged;
		Type typeOfSystem = ((WorldUnmanaged)(ref unmanaged)).GetTypeOfSystem(existingSystem);
		EcsSystemCategory ecsSystemCategory = CategorizeSystem(systemTypeIndex);
		EcsSystemTreeNode result = new EcsSystemTreeNode(ecsSystemCategory, existingSystem, typeOfSystem, world.GetExistingSystemInternal(systemTypeIndex));
		switch (ecsSystemCategory)
		{
		case EcsSystemCategory.Group:
			counts.Group++;
			break;
		case EcsSystemCategory.Base:
			counts.Base++;
			break;
		case EcsSystemCategory.Unmanaged:
			counts.Unmanaged++;
			break;
		}
		return result;
	}

	private EcsSystemCategory CategorizeSystem(SystemTypeIndex systemTypeIndex)
	{
		if (!((SystemTypeIndex)(ref systemTypeIndex)).IsGroup)
		{
			if (!((SystemTypeIndex)(ref systemTypeIndex)).IsManaged)
			{
				return EcsSystemCategory.Unmanaged;
			}
			return EcsSystemCategory.Base;
		}
		return EcsSystemCategory.Group;
	}

	private bool TryGetSystemTypeIndex_ForMissedSystem(World world, SystemHandle systemHandle, out SystemTypeIndex systemTypeIndex, bool logDebug = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		systemTypeIndex = SystemTypeIndex.Null;
		if (((SystemHandle)(ref systemHandle)).Equals(SystemHandle.Null))
		{
			return false;
		}
		WorldUnmanaged unmanaged = world.Unmanaged;
		if (!((WorldUnmanaged)(ref unmanaged)).IsSystemValid(systemHandle))
		{
			return false;
		}
		unmanaged = world.Unmanaged;
		Type typeOfSystem = ((WorldUnmanaged)(ref unmanaged)).GetTypeOfSystem(systemHandle);
		if (typeOfSystem == null)
		{
			return false;
		}
		systemTypeIndex = TypeManager.GetSystemTypeIndex(typeOfSystem);
		if (logDebug)
		{
			bool flag = false;
			Enumerator<SystemTypeIndex> enumerator = TypeManager.GetSystemTypeIndices((WorldSystemFilterFlags)(-1), (WorldSystemFilterFlags)0).GetEnumerator();
			while (enumerator.MoveNext())
			{
				SystemTypeIndex current = enumerator.Current;
				if (((SystemTypeIndex)(ref systemTypeIndex)).Equals(current))
				{
					flag = true;
					break;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder2);
			handler.AppendLiteral("Found missed system ");
			handler.AppendFormatted(typeOfSystem.FullName);
			handler.AppendLiteral(".");
			stringBuilder3.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(73, 1, stringBuilder2);
			handler.AppendLiteral("  SystemTypeIndex.Index: ");
			handler.AppendFormatted(((SystemTypeIndex)(ref systemTypeIndex)).Index);
			handler.AppendLiteral(" (retrieved from TypeManager.GetSystemTypeIndex)");
			stringBuilder4.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(58, 1, stringBuilder2);
			handler.AppendLiteral("  Is SystemTypeIndex in TypeManager.GetSystemTypeIndices: ");
			handler.AppendFormatted(flag);
			stringBuilder5.Append(ref handler);
			if (!flag)
			{
				stringBuilder.Append(" (This might be a bug in the TypeManager)");
			}
			Log.LogDebug((object)stringBuilder.ToString());
		}
		return true;
	}
}
