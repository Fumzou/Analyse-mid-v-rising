using System.Collections.Generic;
using System.Linq;
using System.Text;
using KindredExtract.Models;
using Unity.Entities;

namespace KindredExtract.Services;

internal class EcsSystemDumper
{
	private int _spacesPerIndent;

	public EcsSystemDumper(int spacesPerIndent = 4)
	{
		_spacesPerIndent = spacesPerIndent;
	}

	public string CreateDumpString(EcsSystemHierarchy systemHierarchy)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string value = "\n----------------------------------------\n\n";
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(40, 1, stringBuilder2);
		handler.AppendLiteral("Information about ECS Systems in world: ");
		handler.AppendFormatted(systemHierarchy.World.Name);
		stringBuilder2.AppendLine(ref handler);
		stringBuilder.Append(value);
		AppendSectionCounts(stringBuilder, systemHierarchy);
		if (systemHierarchy.Counts.Unknown > 0 && systemHierarchy.KnownUnknowns.AreKnown())
		{
			stringBuilder.Append(value);
			AppendSectionKnownUnknowns(stringBuilder, systemHierarchy);
		}
		if (systemHierarchy.FindNodesWithMultipleParents().Any())
		{
			stringBuilder.Append(value);
			AppendSectionMultipleParents(stringBuilder, systemHierarchy);
		}
		if (systemHierarchy.RootNodesUnordered.Any())
		{
			stringBuilder.Append(value);
			AppendSectionUpdateHierarchy(stringBuilder, systemHierarchy);
		}
		return stringBuilder.ToString();
	}

	private void AppendSectionCounts(StringBuilder sb, EcsSystemHierarchy systemHierarchy)
	{
		EcsSystemCounts counts = systemHierarchy.Counts;
		sb.AppendLine("[Counts]");
		sb.AppendLine();
		StringBuilder stringBuilder = sb;
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(22, 1, stringBuilder);
		handler.AppendLiteral("ComponentSystemGroup: ");
		handler.AppendFormatted(counts.Group);
		stringBuilder2.AppendLine(ref handler);
		stringBuilder = sb;
		StringBuilder stringBuilder3 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(49, 1, stringBuilder);
		handler.AppendLiteral("ComponentSystemBase (excluding group instances): ");
		handler.AppendFormatted(counts.Base);
		stringBuilder3.AppendLine(ref handler);
		stringBuilder = sb;
		StringBuilder stringBuilder4 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder);
		handler.AppendLiteral("ISystem: ");
		handler.AppendFormatted(counts.Unmanaged);
		stringBuilder4.AppendLine(ref handler);
		stringBuilder = sb;
		StringBuilder stringBuilder5 = stringBuilder;
		handler = new StringBuilder.AppendInterpolatedStringHandler(23, 1, stringBuilder);
		handler.AppendLiteral("<unknown system type>: ");
		handler.AppendFormatted(counts.Unknown);
		stringBuilder5.AppendLine(ref handler);
	}

	private void AppendSectionKnownUnknowns(StringBuilder sb, EcsSystemHierarchy systemHierarchy)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		KnownUnknowns knownUnknowns = systemHierarchy.KnownUnknowns;
		string value = new string(' ', _spacesPerIndent);
		sb.AppendLine("[Known Unknowns]");
		sb.AppendLine();
		sb.AppendLine("Potential reasons for <unknown system type>.");
		sb.AppendLine();
		if (!knownUnknowns.SystemNotFoundInWorld.Any())
		{
			return;
		}
		sb.AppendLine("Issue: SystemHandle found in a group, but no corresponding system in the world");
		foreach (SystemHandle item in knownUnknowns.SystemNotFoundInWorld)
		{
			sb.Append(value);
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, sb);
			handler.AppendFormatted<SystemHandle>(item);
			sb.Append(ref handler);
			sb.AppendLine();
		}
	}

	private void AppendSectionMultipleParents(StringBuilder sb, EcsSystemHierarchy systemHierarchy)
	{
		IList<EcsSystemTreeNode> list = systemHierarchy.FindNodesWithMultipleParents();
		string value = new string(' ', _spacesPerIndent);
		sb.AppendLine("[Systems in multiple groups]");
		sb.AppendLine();
		sb.AppendLine("This probably shouldn't happen!");
		sb.AppendLine();
		foreach (EcsSystemTreeNode item in list)
		{
			StringBuilder stringBuilder = sb;
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(21, 2, stringBuilder);
			handler.AppendFormatted(SystemTypeDescription(item));
			handler.AppendLiteral(" - belongs to ");
			handler.AppendFormatted(item.Parents.Count);
			handler.AppendLiteral(" groups");
			stringBuilder2.AppendLine(ref handler);
			foreach (EcsSystemTreeNode parent in item.Parents)
			{
				sb.Append(value);
				stringBuilder = sb;
				StringBuilder stringBuilder3 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, stringBuilder);
				handler.AppendFormatted(SystemTypeDescription(parent));
				stringBuilder3.Append(ref handler);
				sb.AppendLine();
			}
		}
	}

	private void AppendSectionUpdateHierarchy(StringBuilder sb, EcsSystemHierarchy systemHierarchy)
	{
		sb.AppendLine("[Update Hierarchy]");
		sb.AppendLine();
		sb.AppendLine("The ordering at root level is arbitrary, but everything within a group is in update order for that group.");
		sb.AppendLine();
		foreach (EcsSystemTreeNode item in systemHierarchy.RootNodesUnordered)
		{
			AppendTreeNode(sb, item, 0);
		}
	}

	private void AppendTreeNode(StringBuilder sb, EcsSystemTreeNode node, int depth)
	{
		string value = new string(' ', _spacesPerIndent * depth);
		sb.Append(value);
		sb.Append(SystemDescription(node));
		sb.AppendLine();
		foreach (EcsSystemTreeNode item in node.ChildrenOrderedForUpdate)
		{
			AppendTreeNode(sb, item, depth + 1);
		}
	}

	internal static string SystemDescription(EcsSystemTreeNode node)
	{
		IList<string> list = new List<string>();
		list.Add(SystemTypeDescription(node));
		if (node.Category.Equals(EcsSystemCategory.Group))
		{
			list.Add($"{node.CountDescendants()} descendants");
			list.Add($"{node.ChildrenOrderedForUpdate.Count} children");
		}
		if (node.Parents.Count > 1)
		{
			list.Add($"{node.Parents.Count} parents");
		}
		return string.Join(" | ", list);
	}

	internal static string SystemTypeDescription(EcsSystemTreeNode node)
	{
		return node.Category switch
		{
			EcsSystemCategory.Group => node.Type.FullName + " (ComponentSystemGroup)", 
			EcsSystemCategory.Base => node.Type.FullName + " (ComponentSystemBase)", 
			EcsSystemCategory.Unmanaged => node.Type.FullName + " (ISystem)", 
			_ => "<unknown system type>", 
		};
	}
}
