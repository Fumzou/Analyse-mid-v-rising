using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using ProjectM;
using ProjectM.Network;
using ProjectM.Shared;
using ProjectM.Terrain;
using ProjectM.Tiles;
using Stunlock.Core;
using Stunlock.Localization;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace KindredExtract;

internal class EntityDebug
{
	public delegate string ComponentExtractor(Entity entity);

	private static Dictionary<TypeIndex, ComponentExtractor> componentExtractors = new Dictionary<TypeIndex, ComponentExtractor>();

	public static void RegisterExtractor<T>() where T : struct
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ComponentType val = default(ComponentType);
			((ComponentType)(ref val))._002Ector(Il2CppType.Of<T>(), (AccessMode)0);
			if (((ComponentType)(ref val)).IsZeroSized)
			{
				componentExtractors.Add(val.TypeIndex, (Entity entity) => "  " + typeof(T).ToString() + "\n");
			}
			else if (((ComponentType)(ref val)).IsBuffer)
			{
				componentExtractors.Add(val.TypeIndex, delegate(Entity entity)
				{
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					StringBuilder stringBuilder = new StringBuilder();
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder3 = stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
					handler.AppendLiteral("  ");
					handler.AppendFormatted(typeof(T).ToString());
					stringBuilder3.AppendLine(ref handler);
					T[] array = ReadBuffer<T>(entity);
					for (int i = 0; i < Mathf.Min(array.Length, 300); i++)
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder4 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder2);
						handler.AppendLiteral("   [");
						handler.AppendFormatted(i);
						handler.AppendLiteral("]");
						stringBuilder4.AppendLine(ref handler);
						stringBuilder.AppendLine(RetrieveFields(array[i]));
					}
					if (array.Length > 36)
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder5 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(48, 1, stringBuilder2);
						handler.AppendLiteral("   ");
						handler.AppendFormatted(array.Length);
						handler.AppendLiteral(" total elements but only showing the first 36");
						stringBuilder5.AppendLine(ref handler);
					}
					return stringBuilder.ToString();
				});
			}
			else
			{
				componentExtractors.Add(val.TypeIndex, delegate(Entity entity)
				{
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					StringBuilder stringBuilder2;
					StringBuilder stringBuilder = (stringBuilder2 = new StringBuilder());
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
					handler.AppendLiteral("  ");
					handler.AppendFormatted(typeof(T).ToString());
					stringBuilder2.AppendLine(ref handler);
					stringBuilder.AppendLine(RetrieveFields(entity.Read<T>()));
					return stringBuilder.ToString();
				});
			}
		}
		catch (Exception ex)
		{
			ManualLogSource log = Core.Log;
			bool flag = default(bool);
			BepInExErrorLogInterpolatedStringHandler val2 = new BepInExErrorLogInterpolatedStringHandler(34, 2, ref flag);
			if (flag)
			{
				((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("Failed to register extractor for ");
				((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>(typeof(T).ToString());
				((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("\n");
				((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>(ex.Message);
			}
			log.LogError(val2);
		}
	}

	public static string RetrieveComponentData(Entity entity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		Core.InitializeAfterLoaded();
		EntityManager entityManager = Core.EntityManager;
		StringBuilder stringBuilder = new StringBuilder();
		if (!((EntityManager)(ref entityManager)).Exists(entity))
		{
			stringBuilder.AppendLine("Entity does not exist");
			return stringBuilder.ToString();
		}
		if (entity.Has<User>())
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(17, 3, stringBuilder2);
			handler.AppendLiteral("User ");
			handler.AppendFormatted<FixedString64Bytes>(entity.Read<User>().CharacterName);
			handler.AppendLiteral(" - Entity(");
			handler.AppendFormatted(entity.Index);
			handler.AppendLiteral(":");
			handler.AppendFormatted(entity.Version);
			handler.AppendLiteral(")");
			stringBuilder3.AppendLine(ref handler);
		}
		else if (entity.Has<PlayerCharacter>())
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(19, 3, stringBuilder2);
			handler.AppendLiteral("Player ");
			handler.AppendFormatted<FixedString64Bytes>(entity.Read<PlayerCharacter>().Name);
			handler.AppendLiteral(" - Entity(");
			handler.AppendFormatted(entity.Index);
			handler.AppendLiteral(":");
			handler.AppendFormatted(entity.Version);
			handler.AppendLiteral(")");
			stringBuilder4.AppendLine(ref handler);
		}
		else if (entity.Has<PrefabGUID>())
		{
			if (entity.Has<Prefab>())
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder5 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
				handler.AppendLiteral("Prefab ");
				handler.AppendFormatted(entity.Read<PrefabGUID>().LookupName());
				stringBuilder5.AppendLine(ref handler);
			}
			else
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder6 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(19, 3, stringBuilder2);
				handler.AppendLiteral("Prefab ");
				handler.AppendFormatted(entity.Read<PrefabGUID>().LookupName());
				handler.AppendLiteral(" - Entity(");
				handler.AppendFormatted(entity.Index);
				handler.AppendLiteral(":");
				handler.AppendFormatted(entity.Version);
				handler.AppendLiteral(")");
				stringBuilder6.AppendLine(ref handler);
			}
		}
		else
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder7 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(9, 2, stringBuilder2);
			handler.AppendLiteral("Entity(");
			handler.AppendFormatted(entity.Index);
			handler.AppendLiteral(":");
			handler.AppendFormatted(entity.Version);
			handler.AppendLiteral(")");
			stringBuilder7.AppendLine(ref handler);
		}
		stringBuilder.AppendLine("Components");
		NativeArray<ComponentType> componentTypes = ((EntityManager)(ref entityManager)).GetComponentTypes(entity, (Allocator)4);
		Enumerator<ComponentType> enumerator = componentTypes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ComponentType current = enumerator.Current;
			if (componentExtractors.TryGetValue(current.TypeIndex, out var value))
			{
				try
				{
					stringBuilder.Append(value(entity));
				}
				catch (Exception ex)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder8 = stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(22, 2, stringBuilder2);
					handler.AppendLiteral("  ");
					handler.AppendFormatted<ComponentType>(current);
					handler.AppendLiteral(" failed to extract: ");
					handler.AppendFormatted(ex.Message);
					stringBuilder8.AppendLine(ref handler);
				}
			}
			else
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder9 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder2);
				handler.AppendLiteral("  ");
				handler.AppendFormatted<ComponentType>(current);
				handler.AppendLiteral(" isn't handled");
				stringBuilder9.AppendLine(ref handler);
			}
		}
		componentTypes.Dispose();
		return stringBuilder.ToString();
	}

	public unsafe static string RetrieveFields<T>(T component, string prepend = "    ")
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0909: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0993: Unknown result type (might be due to invalid IL or missing references)
		//IL_0995: Unknown result type (might be due to invalid IL or missing references)
		//IL_099a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_101a: Unknown result type (might be due to invalid IL or missing references)
		//IL_101f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1112: Unknown result type (might be due to invalid IL or missing references)
		//IL_1114: Unknown result type (might be due to invalid IL or missing references)
		//IL_1050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0efb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0add: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_115b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1160: Unknown result type (might be due to invalid IL or missing references)
		//IL_1189: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1222: Unknown result type (might be due to invalid IL or missing references)
		//IL_1224: Unknown result type (might be due to invalid IL or missing references)
		//IL_123c: Unknown result type (might be due to invalid IL or missing references)
		//IL_123e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f39: Unknown result type (might be due to invalid IL or missing references)
		//IL_1282: Unknown result type (might be due to invalid IL or missing references)
		//IL_12da: Unknown result type (might be due to invalid IL or missing references)
		//IL_1332: Unknown result type (might be due to invalid IL or missing references)
		//IL_138d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1392: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1575: Unknown result type (might be due to invalid IL or missing references)
		//IL_157a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1595: Unknown result type (might be due to invalid IL or missing references)
		//IL_1515: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_15dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_167a: Unknown result type (might be due to invalid IL or missing references)
		//IL_167f: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_16aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_15eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1625: Unknown result type (might be due to invalid IL or missing references)
		//IL_1717: Unknown result type (might be due to invalid IL or missing references)
		//IL_171c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1725: Unknown result type (might be due to invalid IL or missing references)
		//IL_172a: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1825: Unknown result type (might be due to invalid IL or missing references)
		//IL_182a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1853: Unknown result type (might be due to invalid IL or missing references)
		//IL_1899: Unknown result type (might be due to invalid IL or missing references)
		//IL_189e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1918: Unknown result type (might be due to invalid IL or missing references)
		//IL_191d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1938: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_18bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1978: Unknown result type (might be due to invalid IL or missing references)
		//IL_197d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1998: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_19dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a58: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b18: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c05: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b77: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b97: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.FieldType == typeof(ModifiableFloat))
			{
				ModifiableFloat val = (ModifiableFloat)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {((ModifiableFloat)(ref val)).Value}");
			}
			else if (fieldInfo.FieldType == typeof(ModifiableInt))
			{
				ModifiableInt val2 = (ModifiableInt)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {((ModifiableInt)(ref val2)).Value}");
			}
			else if (fieldInfo.FieldType == typeof(ModifiableBool))
			{
				ModifiableBool val3 = (ModifiableBool)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {((ModifiableBool)(ref val3)).Value}");
			}
			else if (fieldInfo.FieldType == typeof(ModifiableFloat3))
			{
				ModifiableFloat3 val4 = (ModifiableFloat3)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {((ModifiableFloat3)(ref val4)).Value}");
			}
			else if (fieldInfo.FieldType == typeof(PrefabGUID))
			{
				PrefabGUID prefabGuid = (PrefabGUID)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": " + prefabGuid.LookupName());
			}
			else if (fieldInfo.FieldType == typeof(ModificationData<float>))
			{
				ModificationData<float> val5 = (ModificationData<float>)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {val5.Source} is {((object)(*(ModificationType*)(&val5.ModType))/*cast due to .constrained prefix*/).ToString()} with value {val5.ModValue} to {((object)(*(ModificationId*)(&val5.Id))/*cast due to .constrained prefix*/).ToString()} and priority {val5.Priority}");
			}
			else if (fieldInfo.FieldType == typeof(ModificationData<int>))
			{
				ModificationData<int> val6 = (ModificationData<int>)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {val6.Source} is {((object)(*(ModificationType*)(&val6.ModType))/*cast due to .constrained prefix*/).ToString()} with value {val6.ModValue} to {((object)(*(ModificationId*)(&val6.Id))/*cast due to .constrained prefix*/).ToString()} and priority {val6.Priority}");
			}
			else if (fieldInfo.FieldType == typeof(ModificationData<long>))
			{
				ModificationData<long> val7 = (ModificationData<long>)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {val7.Source} is {((object)(*(ModificationType*)(&val7.ModType))/*cast due to .constrained prefix*/).ToString()} with value {val7.ModValue} to {((object)(*(ModificationId*)(&val7.Id))/*cast due to .constrained prefix*/).ToString()} and priority {val7.Priority}");
			}
			else if (fieldInfo.FieldType == typeof(ModificationData<bool>))
			{
				ModificationData<bool> val8 = (ModificationData<bool>)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {val8.Source} is {((object)(*(ModificationType*)(&val8.ModType))/*cast due to .constrained prefix*/).ToString()} with value {val8.ModValue} to {((object)(*(ModificationId*)(&val8.Id))/*cast due to .constrained prefix*/).ToString()} and priority {val8.Priority}");
			}
			else if (fieldInfo.FieldType == typeof(GameplayEventId))
			{
				GameplayEventId val9 = (GameplayEventId)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {val9.GameplayEventType} - {val9.EventId}");
			}
			else if (fieldInfo.FieldType == typeof(MapZoneId))
			{
				MapZoneId val10 = (MapZoneId)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: MapZoneID(ZoneId: {val10.ZoneId}, ZoneIndex: {((MapZoneId)(ref val10)).ZoneIndex}, Chunk: ({val10.ChunkCoordinate.X}, {val10.ChunkCoordinate.Y}))");
			}
			else if (fieldInfo.FieldType == typeof(BlobAssetReference<ConditionBlob>))
			{
				try
				{
					BlobAssetReference<ConditionBlob> val11 = (BlobAssetReference<ConditionBlob>)fieldInfo.GetValue(component);
					if (val11.IsCreated)
					{
						ConditionBlob ptr = (ConditionBlob)(*val11.m_data.m_Ptr);
						stringBuilder.AppendLine(prepend + fieldInfo.Name + ": ConditionBlob");
						stringBuilder.AppendLine(prepend + " ConditionInfo");
						stringBuilder.AppendLine(prepend + "  Prefab " + ((BlobString)(ref ptr.Info.Prefab)).ToString());
						stringBuilder.AppendLine(prepend + "  Component " + ((BlobString)(ref ptr.Info.Component)).ToString());
						stringBuilder.AppendLine(prepend + " ConditionalElements");
					}
					else
					{
						stringBuilder.AppendLine(prepend + fieldInfo.Name + ": None");
					}
				}
				catch
				{
					stringBuilder.AppendLine(prepend + fieldInfo.Name + ": " + fieldInfo.GetValue(component).ToString());
				}
			}
			else if (fieldInfo.FieldType == typeof(BlobString))
			{
				BlobString val12 = (BlobString)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": " + ((BlobString)(ref val12)).ToString());
			}
			else if (fieldInfo.FieldType == typeof(Entity))
			{
				Entity val13 = (Entity)fieldInfo.GetValue(component);
				string text = $"Entity({val13.Index}:{val13.Version})";
				if (val13.Has<User>())
				{
					stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: User {val13.Read<User>().CharacterName} - {text}");
				}
				else if (val13.Has<PlayerCharacter>())
				{
					stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: Player {val13.Read<PlayerCharacter>().Name} - {text}");
				}
				else if (val13.Has<PrefabGUID>())
				{
					if (val13.Has<Prefab>())
					{
						stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: Prefab {val13.Read<PrefabGUID>().LookupName()} - {text}");
					}
					else
					{
						stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: Entity {val13.Read<PrefabGUID>().LookupName()} - {text}");
					}
				}
				else
				{
					stringBuilder.AppendLine(prepend + fieldInfo.Name + ": " + text);
				}
			}
			else if (fieldInfo.FieldType == typeof(NetworkedEntity))
			{
				NetworkedEntity val14 = (NetworkedEntity)fieldInfo.GetValue(component);
				Entity entityOnServer = ((NetworkedEntity)(ref val14)).GetEntityOnServer();
				string text2 = $"NetworkedEntity({entityOnServer.Index}:{entityOnServer.Version})";
				if (entityOnServer.Has<User>())
				{
					stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: User {entityOnServer.Read<User>().CharacterName} - {text2}");
				}
				else if (entityOnServer.Has<PlayerCharacter>())
				{
					stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: Player {entityOnServer.Read<PlayerCharacter>().Name} - {text2}");
				}
				else if (entityOnServer.Has<PrefabGUID>())
				{
					if (entityOnServer.Has<Prefab>())
					{
						stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: Prefab {entityOnServer.Read<PrefabGUID>().LookupName()} - {text2}");
					}
					else
					{
						stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: Entity {entityOnServer.Read<PrefabGUID>().LookupName()} - {text2}");
					}
				}
				else
				{
					stringBuilder.AppendLine(prepend + fieldInfo.Name + ": " + text2);
				}
			}
			else if (fieldInfo.FieldType == typeof(ModifiableEntity))
			{
				ModifiableEntity val15 = (ModifiableEntity)fieldInfo.GetValue(component);
				Entity value = ((ModifiableEntity)(ref val15)).Value;
				string text3 = $"ModifiableEntity({value.Index}:{value.Version})";
				if (value.Has<User>())
				{
					stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: User {value.Read<User>().CharacterName} - {text3}");
				}
				else if (value.Has<PlayerCharacter>())
				{
					stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: Player {value.Read<PlayerCharacter>().Name} - {text3}");
				}
				else if (value.Has<PrefabGUID>())
				{
					if (value.Has<Prefab>())
					{
						stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: Prefab {value.Read<PrefabGUID>().LookupName()} - {text3}");
					}
					else
					{
						stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: Entity {value.Read<PrefabGUID>().LookupName()} - {text3}");
					}
				}
				else
				{
					stringBuilder.AppendLine(prepend + fieldInfo.Name + ": " + text3);
				}
			}
			else if (fieldInfo.FieldType == typeof(ModificationId))
			{
				ModificationId val16 = (ModificationId)fieldInfo.GetValue(component);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
				defaultInterpolatedStringHandler.AppendFormatted(fieldInfo.Name);
				defaultInterpolatedStringHandler.AppendLiteral(": ");
				defaultInterpolatedStringHandler.AppendFormatted(((ModificationId)(ref val16)).IsEmpty() ? "Unset" : ((object)val16.Id));
				stringBuilder.AppendLine(prepend + defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else if (fieldInfo.FieldType == typeof(CreateGameplayEventsOnSpawn))
			{
				CreateGameplayEventsOnSpawn val17 = (CreateGameplayEventsOnSpawn)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {val17.EventId.EventId} {val17.EventId.GameplayEventType} - {val17.Target}");
			}
			else if (fieldInfo.FieldType == typeof(GameplayEventId))
			{
				GameplayEventId val18 = (GameplayEventId)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: (EventId: {val18.EventId}, GameplayEventType: {val18.GameplayEventType})");
			}
			else if (fieldInfo.FieldType == typeof(TilePivotSettings))
			{
				TilePivotSettings val19 = (TilePivotSettings)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {val19.PivotType} - {val19.CustomPivotPoint}");
			}
			else if (fieldInfo.FieldType == typeof(TileDatas<CollisionData>))
			{
				_ = (TileDatas<CollisionData>)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": [");
				stringBuilder.AppendLine(prepend + "]");
			}
			else if (fieldInfo.FieldType == typeof(TileDatas<TileHeightData>))
			{
				_ = (TileDatas<TileHeightData>)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": [");
				stringBuilder.AppendLine(prepend + "]");
			}
			else if (fieldInfo.FieldType == typeof(TileDatas<SurfaceFluffData>))
			{
				_ = (TileDatas<SurfaceFluffData>)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": [");
				stringBuilder.AppendLine(prepend + "]");
			}
			else if (fieldInfo.FieldType == typeof(BlobAssetReference<WallpaperStyleBlob>))
			{
				BlobAssetReference<WallpaperStyleBlob> val20 = (BlobAssetReference<WallpaperStyleBlob>)fieldInfo.GetValue(component);
				if (val20.IsCreated)
				{
					WallpaperStyleBlob ptr2 = (WallpaperStyleBlob)(*val20.m_data.m_Ptr);
					stringBuilder.AppendLine(prepend + fieldInfo.Name + ": WallpaperStyleBlob");
					stringBuilder.AppendLine(prepend + "  Styles: [");
					stringBuilder.AppendLine(prepend + $"    {ptr2.Styles.Length} entries");
					stringBuilder.AppendLine(prepend + "  ]");
					stringBuilder.AppendLine(prepend + "  MeshVariationsByIndex: [");
					stringBuilder.AppendLine(prepend + $"    {ptr2.MeshVariationsByIndex.Length} entries");
					stringBuilder.AppendLine(prepend + "  ]");
				}
				else
				{
					stringBuilder.AppendLine(prepend + fieldInfo.Name + ": None");
				}
			}
			else if (fieldInfo.FieldType == typeof(Nullable_Unboxed<HeightPlacementConfig>))
			{
				Nullable_Unboxed<HeightPlacementConfig> val21 = (Nullable_Unboxed<HeightPlacementConfig>)fieldInfo.GetValue(component);
				if (val21.HasValue)
				{
					stringBuilder.AppendLine(prepend + fieldInfo.Name + ":");
					stringBuilder.AppendLine(RetrieveFields<HeightPlacementConfig>(val21.Value, prepend + "  "));
				}
				else
				{
					stringBuilder.AppendLine(prepend + fieldInfo.Name + ": None");
				}
			}
			else if (fieldInfo.FieldType == typeof(WallpaperOrientation))
			{
				WallpaperOrientation component2 = (WallpaperOrientation)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ":");
				stringBuilder.Append(RetrieveFields<WallpaperOrientation>(component2, prepend + "  "));
			}
			else if (fieldInfo.FieldType == typeof(BlobAssetReference<SpawnChainBlobAsset>))
			{
				BlobAssetReference<SpawnChainBlobAsset> val22 = (BlobAssetReference<SpawnChainBlobAsset>)fieldInfo.GetValue(component);
				if (val22.IsCreated)
				{
					SpawnChainBlobAsset ptr3 = (SpawnChainBlobAsset)(*val22.m_data.m_Ptr);
					stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: SpawnChainBlobAsset(main {ptr3.MainElementIndex} out of)");
				}
			}
			else if (fieldInfo.FieldType == typeof(Aabb))
			{
				Aabb val23 = (Aabb)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: Aaab({val23.Min} to {val23.Max})");
			}
			else if (fieldInfo.FieldType == typeof(LocalizationKey))
			{
				LocalizationKey val24 = (LocalizationKey)fieldInfo.GetValue(component);
				string text4 = ((object)((AssetGuid)(ref val24.Key)).ToGuid()/*cast due to .constrained prefix*/).ToString();
				string localization = Core.Localization.GetLocalization(text4);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {text4} - {localization}");
			}
			else if (fieldInfo.FieldType == typeof(EquipmentSlot))
			{
				EquipmentSlot component3 = (EquipmentSlot)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ":");
				stringBuilder.AppendLine(RetrieveFields<EquipmentSlot>(component3, prepend + "  "));
			}
			else if (fieldInfo.FieldType == typeof(SequenceGUID))
			{
				SequenceGUID val25 = (SequenceGUID)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: SequenceGUID {val25.GuidHash}");
			}
			else if (fieldInfo.FieldType == typeof(BlobAssetReference<SpellModSetGeneratorBlob>))
			{
				BlobAssetReference<SpellModSetGeneratorBlob> val26 = (BlobAssetReference<SpellModSetGeneratorBlob>)fieldInfo.GetValue(component);
				if (val26.IsCreated)
				{
					SpellModSetGeneratorBlob ptr4 = (SpellModSetGeneratorBlob)(*val26.m_data.m_Ptr);
					stringBuilder.AppendLine(prepend + fieldInfo.Name + ": SpellModSetGeneratorBlob");
					stringBuilder.Append(RetrieveFields<SpellModSetGeneratorBlob>(ptr4, prepend + "  "));
				}
			}
			else if (fieldInfo.FieldType == typeof(GenerateSpellModSetInput))
			{
				GenerateSpellModSetInput component4 = (GenerateSpellModSetInput)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": GenerateSpellModSetInput");
				stringBuilder.AppendLine(RetrieveFields<GenerateSpellModSetInput>(component4, prepend + "  "));
			}
			else if (fieldInfo.FieldType == typeof(SpellModSet))
			{
				SpellModSet component5 = (SpellModSet)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": SpellModSet");
				stringBuilder.AppendLine(RetrieveFields<SpellModSet>(component5, prepend + "  "));
			}
			else if (fieldInfo.FieldType == typeof(SpellMod))
			{
				SpellMod component6 = (SpellMod)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": SpellMod");
				stringBuilder.AppendLine(RetrieveFields<SpellMod>(component6, prepend + "  "));
			}
			else if (fieldInfo.FieldType == typeof(DurabilityDamageModifiers))
			{
				DurabilityDamageModifiers component7 = (DurabilityDamageModifiers)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": DurabilityDamageModifiers");
				stringBuilder.AppendLine(RetrieveFields<DurabilityDamageModifiers>(component7, prepend + "  "));
			}
			else if (fieldInfo.FieldType == typeof(Item_DurabilitySettings))
			{
				Item_DurabilitySettings component8 = (Item_DurabilitySettings)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": Item_DurabilitySettings");
				stringBuilder.AppendLine(RetrieveFields<Item_DurabilitySettings>(component8, prepend + "  "));
			}
			else if (fieldInfo.FieldType == typeof(SequenceState))
			{
				SequenceState component9 = (SequenceState)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + fieldInfo.Name + ": SequenceState");
				stringBuilder.AppendLine(RetrieveFields<SequenceState>(component9, prepend + "  "));
			}
			else if (fieldInfo.FieldType == typeof(BlobAssetReference<StaticHierarchyBlobAsset>))
			{
				BlobAssetReference<StaticHierarchyBlobAsset> val27 = (BlobAssetReference<StaticHierarchyBlobAsset>)fieldInfo.GetValue(component);
				if (val27.IsCreated)
				{
					StaticHierarchyBlobAsset ptr5 = (StaticHierarchyBlobAsset)(*val27.m_data.m_Ptr);
					stringBuilder.AppendLine(prepend + fieldInfo.Name + ": StaticHierarchyBlobAsset");
					stringBuilder.Append(RetrieveFields<StaticHierarchyBlobAsset>(ptr5, prepend + "  "));
				}
			}
			else if (fieldInfo.FieldType == typeof(ModifiablePrefabGUID))
			{
				ModifiablePrefabGUID val28 = (ModifiablePrefabGUID)fieldInfo.GetValue(component);
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: ModifiablePrefabGUID {val28._Value}");
			}
			else if (fieldInfo.FieldType.AssemblyQualifiedName.StartsWith("System"))
			{
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {fieldInfo.GetValue(component)}");
			}
			else
			{
				stringBuilder.AppendLine(prepend + $"{fieldInfo.Name}: {fieldInfo.FieldType} {fieldInfo.GetValue(component)}");
			}
		}
		return stringBuilder.ToString();
	}

	private static T[] ReadBuffer<T>(Entity entity) where T : struct
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		List<T> list = new List<T>();
		EntityManager entityManager;
		try
		{
			entityManager = Core.EntityManager;
			Enumerator<T> enumerator = ((EntityManager)(ref entityManager)).GetBuffer<T>(entity, false).GetEnumerator();
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				list.Add(current);
			}
		}
		catch (Exception)
		{
			try
			{
				entityManager = Core.EntityManager;
				Enumerator<T> enumerator = ((EntityManager)(ref entityManager)).GetBufferReadOnly<T>(entity).GetEnumerator();
				while (enumerator.MoveNext())
				{
					T current2 = enumerator.Current;
					list.Add(current2);
				}
			}
			catch (Exception)
			{
			}
		}
		return list.ToArray();
	}
}
