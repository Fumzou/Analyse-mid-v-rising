using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using ProjectM;
using ProjectM.Shared;
using ProjectM.UI;
using Stunlock.Core;
using Stunlock.Localization;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using VampireCommandFramework;

namespace KindredExtract.Commands;

[CommandGroup("dump", null)]
internal class DumpCommands
{
	private class ArchetypeInfo
	{
		public int Hash { get; set; }

		public ComponentType[] ComponentTypes { get; set; }

		public int EntityCount { get; set; }
	}

	[Command("prefabs", "p", null, "Dumps all prefabs to a file as prefabGuids for a Prefabs.cs file", null, true)]
	public static void DumpPrefabs(ChatCommandContext ctx)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		HashSet<string> hashSet = new HashSet<string>();
		List<string> list = new List<string>();
		PrefabCollectionSystem existingSystemManaged = Core.TheWorld.GetExistingSystemManaged<PrefabCollectionSystem>();
		NativeParallelHashMap<PrefabGUID, Entity> guidToEntityMap = existingSystemManaged._PrefabLookupMap.GuidToEntityMap;
		Enumerator<PrefabGUID> enumerator = guidToEntityMap.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
		while (enumerator.MoveNext())
		{
			PrefabGUID current = enumerator.Current;
			PrefabLookupMap prefabLookupMap = existingSystemManaged._PrefabLookupMap;
			string name = ((PrefabLookupMap)(ref prefabLookupMap)).GetName(current);
			name = name.Replace(" ", "_").Replace(".", "_").Replace("-", "_")
				.Replace("(", "_")
				.Replace(")", "_");
			string value = name;
			int num = 2;
			while (hashSet.Contains(name))
			{
				name = $"{value}_ALREADY_EXISTS_{num++}";
			}
			hashSet.Add(name);
			list.Add($"\tpublic static readonly PrefabGUID {name} = new PrefabGUID({((PrefabGUID)(ref current)).GuidHash});");
		}
		list.Sort();
		File.WriteAllLines("prefabs.txt", list);
		ctx.Reply($"Dumped {list.Count} prefabs to prefabs.txt");
	}

	[Command("types", "t", null, "Dumps all ECS component types to file (for usage in ComponentExtractors.tt)", null, true)]
	public static void DumpComponentTypes(ChatCommandContext ctx)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Il2CppStructArray<TypeInfo> allTypes = TypeManager.GetAllTypes();
		string text = "componentTypes.txt";
		int num = 0;
		using (StreamWriter streamWriter = new StreamWriter(text))
		{
			foreach (TypeInfo item in (Il2CppArrayBase<TypeInfo>)(object)allTypes)
			{
				if (!(item.TypeIndex == TypeIndex.op_Implicit(0)))
				{
					Type type = TypeManager.GetType(item.TypeIndex);
					if (!(type == (Type)null) && !type.IsClass && !type.IsEnum && type.IsValueType)
					{
						streamWriter.WriteLine("    \"" + type.FullName.Replace("+", ".") + "\",");
						num++;
					}
				}
			}
		}
		ctx.Reply($"Dumped {num} component types to {text}");
	}

	[Command("entityqueries", "eq", null, "Dumps all ECS entity queries to file", null, true)]
	public static void DumpEntityQueries(ChatCommandContext ctx)
	{
		StringBuilder stringBuilder = new StringBuilder();
		SystemsQueryExtraction.DumpAllSystemQueries(stringBuilder);
		File.WriteAllText("EntityQueryDescriptions.txt", stringBuilder.ToString());
	}

	public unsafe static void DumpSystemQueries<T>(T system, StringBuilder sb) where T : ComponentSystemBase
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		if (system == null)
		{
			return;
		}
		Type type = ((object)system).GetType();
		IEnumerable<FieldInfo> enumerable = from p in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where p.FieldType == typeof(EntityQuery)
			select p;
		IEnumerable<PropertyInfo> enumerable2 = from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where p.PropertyType == typeof(EntityQuery)
			select p;
		if (!enumerable.Any() && !enumerable2.Any())
		{
			return;
		}
		StringBuilder stringBuilder = sb;
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder);
		handler.AppendLiteral("System: ");
		handler.AppendFormatted(((object)system).GetType().FullName);
		stringBuilder2.AppendLine(ref handler);
		foreach (FieldInfo item in enumerable)
		{
			try
			{
				EntityQuery val = (EntityQuery)item.GetValue(system);
				stringBuilder = sb;
				StringBuilder stringBuilder3 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder);
				handler.AppendLiteral("  EntityQuery Field: ");
				handler.AppendFormatted(item.Name);
				stringBuilder3.AppendLine(ref handler);
				if (val == default(EntityQuery))
				{
					sb.AppendLine("    Invalid to use");
					continue;
				}
				EntityQueryDesc entityQueryDesc = ((EntityQuery)(ref val)).GetEntityQueryDesc();
				stringBuilder = sb;
				StringBuilder stringBuilder4 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(23, 1, stringBuilder);
				handler.AppendLiteral("    Absent Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc.Absent).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder4.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder5 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(20, 1, stringBuilder);
				handler.AppendLiteral("    All Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc.All).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder5.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder6 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(20, 1, stringBuilder);
				handler.AppendLiteral("    Any Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc.Any).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder6.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder7 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(25, 1, stringBuilder);
				handler.AppendLiteral("    Disabled Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc.Disabled).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder7.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder8 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder);
				handler.AppendLiteral("    None Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc.None).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder8.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder9 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(24, 1, stringBuilder);
				handler.AppendLiteral("    Present Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc.Present).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder9.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder10 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder);
				handler.AppendLiteral("    Options: ");
				handler.AppendFormatted<EntityQueryOptions>(entityQueryDesc.Options);
				stringBuilder10.AppendLine(ref handler);
			}
			catch (Exception)
			{
				sb.AppendLine("    Invalid to use");
			}
		}
		foreach (PropertyInfo item2 in enumerable2)
		{
			try
			{
				EntityQuery val2 = (EntityQuery)item2.GetValue(system);
				stringBuilder = sb;
				StringBuilder stringBuilder11 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(24, 1, stringBuilder);
				handler.AppendLiteral("  EntityQuery Property: ");
				handler.AppendFormatted(item2.Name);
				stringBuilder11.AppendLine(ref handler);
				if (val2 == default(EntityQuery))
				{
					sb.AppendLine("    Invalid to use");
					continue;
				}
				EntityQueryDesc entityQueryDesc2 = ((EntityQuery)(ref val2)).GetEntityQueryDesc();
				stringBuilder = sb;
				StringBuilder stringBuilder12 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(23, 1, stringBuilder);
				handler.AppendLiteral("    Absent Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc2.Absent).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder12.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder13 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(20, 1, stringBuilder);
				handler.AppendLiteral("    All Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc2.All).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder13.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder14 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(20, 1, stringBuilder);
				handler.AppendLiteral("    Any Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc2.Any).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder14.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder15 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(25, 1, stringBuilder);
				handler.AppendLiteral("    Disabled Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc2.Disabled).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder15.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder16 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder);
				handler.AppendLiteral("    None Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc2.None).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder16.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder17 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(24, 1, stringBuilder);
				handler.AppendLiteral("    Present Components: ");
				handler.AppendFormatted(string.Join(", ", ((IEnumerable<ComponentType>)entityQueryDesc2.Present).Select((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString())));
				stringBuilder17.AppendLine(ref handler);
				stringBuilder = sb;
				StringBuilder stringBuilder18 = stringBuilder;
				handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder);
				handler.AppendLiteral("    Options: ");
				handler.AppendFormatted<EntityQueryOptions>(entityQueryDesc2.Options);
				stringBuilder18.AppendLine(ref handler);
			}
			catch (Exception)
			{
				sb.AppendLine("    Invalid to use");
			}
		}
		sb.AppendLine();
		sb.AppendLine();
	}

	[Command("prefabjsons", "pj", null, "Dumps all prefab names and ids to JSON files, grouped by prefix", null, true)]
	public static void DumpPrefabJsons(ChatCommandContext ctx)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		PrefabCollectionSystem existingSystemManaged = Core.TheWorld.GetExistingSystemManaged<PrefabCollectionSystem>();
		Dictionary<string, Dictionary<string, int>> dictionary = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
		NativeParallelHashMap<PrefabGUID, Entity> guidToEntityMap = existingSystemManaged._PrefabLookupMap.GuidToEntityMap;
		Enumerator<PrefabGUID> enumerator = guidToEntityMap.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
		while (enumerator.MoveNext())
		{
			PrefabGUID current = enumerator.Current;
			PrefabLookupMap prefabLookupMap = existingSystemManaged._PrefabLookupMap;
			string name = ((PrefabLookupMap)(ref prefabLookupMap)).GetName(current);
			string text = name.Split('_')[0];
			bool flag = false;
			for (int i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(name[i]))
				{
					if (flag)
					{
						text = name.Substring(0, i);
						break;
					}
				}
				else
				{
					flag = true;
				}
			}
			if (!dictionary.TryGetValue(text, out var value))
			{
				value = (dictionary[text] = new Dictionary<string, int>());
			}
			value[name] = ((PrefabGUID)(ref current)).GuidHash;
		}
		if (!Directory.Exists("prefabJsons"))
		{
			Directory.CreateDirectory("prefabJsons");
		}
		foreach (KeyValuePair<string, Dictionary<string, int>> item in dictionary)
		{
			item.Deconstruct(out var key, out var value2);
			string text2 = key;
			string contents = JsonSerializer.Serialize(value2.OrderBy((KeyValuePair<string, int> x) => x.Key).ToDictionary((KeyValuePair<string, int> x) => x.Key, (KeyValuePair<string, int> x) => x.Value), new JsonSerializerOptions
			{
				WriteIndented = true
			});
			File.WriteAllText("prefabJsons/" + text2 + ".json", contents);
		}
		ctx.Reply($"Dumped {dictionary.Count} JSON files to prefabJsons folder");
	}

	[Command("guidpos", null, null, null, null, true)]
	public static void DumpGuidPos(ChatCommandContext ctx, int prefab)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		using StreamWriter streamWriter = new StreamWriter($"{prefab}.csv");
		streamWriter.WriteLine("x,y,z");
		Enumerator<Entity> enumerator = Helper.GetEntitiesByComponentType<PrefabGUID>().GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			PrefabGUID val = current.Read<PrefabGUID>();
			if (((PrefabGUID)(ref val)).GuidHash == prefab && current.Has<Translation>())
			{
				Translation val2 = current.Read<Translation>();
				streamWriter.WriteLine($"{val2.Value.x},{val2.Value.y},{val2.Value.z}");
			}
		}
	}

	[Command("localization", null, null, null, null, true)]
	public static void DumpLocalization(ChatCommandContext ctx)
	{
		Core.Localization.SaveLocalization();
	}

	[Command("prefabnames", null, null, null, null, true)]
	public static void DumpPrefabNames(ChatCommandContext ctx)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Expected O, but got Unknown
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Expected O, but got Unknown
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Expected O, but got Unknown
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Expected O, but got Unknown
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Expected O, but got Unknown
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Expected O, but got Unknown
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Expected O, but got Unknown
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Expected O, but got Unknown
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Expected O, but got Unknown
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		PrefabCollectionSystem existingSystemManaged = Core.TheWorld.GetExistingSystemManaged<PrefabCollectionSystem>();
		GameDataSystem existingSystemManaged2 = Core.TheWorld.GetExistingSystemManaged<GameDataSystem>();
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		Enumerator<PrefabGUID, Entity> enumerator = existingSystemManaged._PrefabGuidToEntityMap.GetEnumerator();
		bool flag = default(bool);
		while (enumerator.MoveNext())
		{
			PrefabGUID key = enumerator.Current.Key;
			ManagedCharacterHUD orDefaultWithoutLogging = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedCharacterHUD>(key, (ManagedCharacterHUD)null);
			LocalizationKey val2;
			if (orDefaultWithoutLogging != null)
			{
				ManualLogSource log = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(17, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" ManagedCharacter");
				}
				log.LogInfo(val);
				int guidHash = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging.Name;
				dictionary[guidHash] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedItemData orDefaultWithoutLogging2 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedItemData>(key, (ManagedItemData)null);
			if (orDefaultWithoutLogging2 != null)
			{
				ManualLogSource log2 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(16, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedItemData");
				}
				log2.LogInfo(val);
				int guidHash2 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging2.Name;
				dictionary[guidHash2] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedUnitBloodTypeData orDefaultWithoutLogging3 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedUnitBloodTypeData>(key, (ManagedUnitBloodTypeData)null);
			if (orDefaultWithoutLogging3 != null)
			{
				ManualLogSource log3 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(21, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedUnitBloodType");
				}
				log3.LogInfo(val);
				int guidHash3 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging3.Name;
				dictionary[guidHash3] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedMissionData orDefaultWithoutLogging4 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedMissionData>(key, (ManagedMissionData)null);
			if (orDefaultWithoutLogging4 != null)
			{
				ManualLogSource log4 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(19, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedMissionData");
				}
				log4.LogInfo(val);
				int guidHash4 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging4.Name;
				dictionary[guidHash4] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedTechData orDefaultWithoutLogging5 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedTechData>(key, (ManagedTechData)null);
			if (orDefaultWithoutLogging5 != null)
			{
				ManualLogSource log5 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(16, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedTechData");
				}
				log5.LogInfo(val);
				int guidHash5 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging5.NameKey;
				dictionary[guidHash5] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedPerkData orDefaultWithoutLogging6 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedPerkData>(key, (ManagedPerkData)null);
			if (orDefaultWithoutLogging6 != null)
			{
				ManualLogSource log6 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(16, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedPerkData");
				}
				log6.LogInfo(val);
				int guidHash6 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging6.Name;
				dictionary[guidHash6] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedBlueprintData orDefaultWithoutLogging7 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedBlueprintData>(key, (ManagedBlueprintData)null);
			if (orDefaultWithoutLogging7 != null)
			{
				ManualLogSource log7 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(21, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedBlueprintData");
				}
				log7.LogInfo(val);
				int guidHash7 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging7.Name;
				dictionary[guidHash7] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedAbilityGroupData orDefaultWithoutLogging8 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedAbilityGroupData>(key, (ManagedAbilityGroupData)null);
			if (orDefaultWithoutLogging8 != null)
			{
				ManualLogSource log8 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(24, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedAbilityGroupData");
				}
				log8.LogInfo(val);
				int guidHash8 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging8.Name;
				dictionary[guidHash8] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedDataDropGroup orDefaultWithoutLogging9 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedDataDropGroup>(key, (ManagedDataDropGroup)null);
			if (orDefaultWithoutLogging9 != null)
			{
				ManualLogSource log9 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(21, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedDataDropGroup");
				}
				log9.LogInfo(val);
				int guidHash9 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging9.Name;
				dictionary[guidHash9] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			if (existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedBuildMenuTagData>(key, (ManagedBuildMenuTagData)null) != null)
			{
				ManualLogSource log10 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(24, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedBuildMenuTagData");
				}
				log10.LogInfo(val);
				int guidHash10 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging7.Name;
				dictionary[guidHash10] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedBuildMenuGroupData orDefaultWithoutLogging10 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedBuildMenuGroupData>(key, (ManagedBuildMenuGroupData)null);
			if (orDefaultWithoutLogging10 != null)
			{
				ManualLogSource log11 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(26, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedBuildMenuGroupData");
				}
				log11.LogInfo(val);
				int guidHash11 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging10.Name;
				dictionary[guidHash11] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedBuildMenuCategoryData orDefaultWithoutLogging11 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedBuildMenuCategoryData>(key, (ManagedBuildMenuCategoryData)null);
			if (orDefaultWithoutLogging11 != null)
			{
				ManualLogSource log12 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(29, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedBuildMenuCategoryData");
				}
				log12.LogInfo(val);
				int guidHash12 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging11.Name;
				dictionary[guidHash12] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				continue;
			}
			ManagedSpellSchoolData orDefaultWithoutLogging12 = existingSystemManaged2.ManagedDataRegistry.GetOrDefaultWithoutLogging<ManagedSpellSchoolData>(key, (ManagedSpellSchoolData)null);
			if (orDefaultWithoutLogging12 != null)
			{
				ManualLogSource log13 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(23, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" managedSpellSchoolData");
				}
				log13.LogInfo(val);
				int guidHash13 = ((PrefabGUID)(ref key)).GuidHash;
				val2 = orDefaultWithoutLogging12.LongName;
				dictionary[guidHash13] = ((object)((AssetGuid)(ref val2.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
			}
		}
		string contents = JsonSerializer.Serialize(dictionary);
		File.WriteAllText("PrefabNames.json", contents);
	}

	[Command("systems", "s", null, "Dumps ECS system update hierarchies to files (per world)", null, true)]
	public static void DumpSystemsUpdateTrees(ChatCommandContext ctx)
	{
		string text = Core.EcsSystemDumpService.DumpSystemsUpdateTrees();
		ctx.Reply("Dumped system hierarchy files to folder " + text);
	}

	[Command("archetypes", "a", null, "Dumps all ECS archetypes to file", null, true)]
	public unsafe static void DumpArchetypes(ChatCommandContext ctx)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		EntityManager entityManager = Core.EntityManager;
		stringBuilder.AppendLine("ECS Archetypes Dump");
		stringBuilder.AppendLine("===================");
		stringBuilder.AppendLine();
		NativeArray<Entity> allEntities = ((EntityManager)(ref entityManager)).GetAllEntities((Allocator)2);
		Dictionary<int, ArchetypeInfo> dictionary = new Dictionary<int, ArchetypeInfo>();
		Enumerator<Entity> enumerator = allEntities.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			ArchetypeChunk chunk = ((EntityManager)(ref entityManager)).GetChunk(current);
			EntityArchetype archetype = ((ArchetypeChunk)(ref chunk)).Archetype;
			int hashCode = ((object)(*(EntityArchetype*)(&archetype))/*cast due to .constrained prefix*/).GetHashCode();
			if (!dictionary.ContainsKey(hashCode))
			{
				NativeArray<ComponentType> componentTypes = ((EntityArchetype)(ref archetype)).GetComponentTypes((Allocator)2);
				dictionary[hashCode] = new ArchetypeInfo
				{
					Hash = hashCode,
					ComponentTypes = Il2CppArrayBase<ComponentType>.op_Implicit(componentTypes.ToArray()),
					EntityCount = 0
				};
				componentTypes.Dispose();
			}
			dictionary[hashCode].EntityCount++;
		}
		allEntities.Dispose();
		List<ArchetypeInfo> list = dictionary.Values.OrderByDescending((ArchetypeInfo a) => a.EntityCount).ToList();
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(18, 1, stringBuilder2);
		handler.AppendLiteral("Total Archetypes: ");
		handler.AppendFormatted(list.Count);
		stringBuilder3.AppendLine(ref handler);
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder4 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder2);
		handler.AppendLiteral("Total Entities: ");
		handler.AppendFormatted(list.Sum((ArchetypeInfo a) => a.EntityCount));
		stringBuilder4.AppendLine(ref handler);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("----------------------------------------");
		stringBuilder.AppendLine();
		int num = 1;
		foreach (ArchetypeInfo item in list)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(20, 2, stringBuilder2);
			handler.AppendLiteral("Archetype #");
			handler.AppendFormatted(num);
			handler.AppendLiteral(" (Hash: ");
			handler.AppendFormatted(item.Hash);
			handler.AppendLiteral(")");
			stringBuilder5.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder6 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder2);
			handler.AppendLiteral("  Entity Count: ");
			handler.AppendFormatted(item.EntityCount);
			stringBuilder6.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder7 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(19, 1, stringBuilder2);
			handler.AppendLiteral("  Component Count: ");
			handler.AppendFormatted(item.ComponentTypes.Length);
			stringBuilder7.AppendLine(ref handler);
			stringBuilder.AppendLine("  Components:");
			foreach (ComponentType item2 in item.ComponentTypes.OrderBy((ComponentType c) => ((object)(*(ComponentType*)(&c))/*cast due to .constrained prefix*/).ToString()))
			{
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder8 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
				handler.AppendLiteral("    - ");
				handler.AppendFormatted<ComponentType>(item2);
				stringBuilder8.AppendLine(ref handler);
			}
			stringBuilder.AppendLine();
			num++;
		}
		string text = "archetypes.txt";
		File.WriteAllText(text, stringBuilder.ToString());
		ctx.Reply($"Dumped {list.Count} archetypes to {text}");
	}
}
