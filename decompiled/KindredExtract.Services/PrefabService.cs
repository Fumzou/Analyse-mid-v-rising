using System.Collections.Generic;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using Il2CppSystem.Collections.Generic;
using ProjectM;
using Stunlock.Core;

namespace KindredExtract.Services;

internal class PrefabService
{
	internal Dictionary<string, (string Name, PrefabGUID Prefab)> NameToGuid { get; init; } = new Dictionary<string, (string, PrefabGUID)>();

	internal PrefabService()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		Dictionary<string, PrefabGUID> spawnableNameToPrefabGuidDictionary = Core.TheWorld.GetExistingSystemManaged<PrefabCollectionSystem>().SpawnableNameToPrefabGuidDictionary;
		ManualLogSource log = Core.Log;
		bool flag = default(bool);
		BepInExDebugLogInterpolatedStringHandler val = new BepInExDebugLogInterpolatedStringHandler(19, 1, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Spawnable prefabs: ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<int>(spawnableNameToPrefabGuidDictionary.Count);
		}
		log.LogDebug(val);
		Enumerator<string, PrefabGUID> enumerator = spawnableNameToPrefabGuidDictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, PrefabGUID> current = enumerator.Current;
			if (!NameToGuid.TryAdd(current.Key.ToLowerInvariant(), (current.Key, current.Value)))
			{
				ManualLogSource log2 = Core.Log;
				val = new BepInExDebugLogInterpolatedStringHandler(25, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(current.Key);
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" exist already, skipping.");
				}
				log2.LogDebug(val);
			}
		}
	}

	internal bool TryGetItem(string input, out PrefabGUID prefab)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		string text = input.ToLowerInvariant();
		(string, PrefabGUID) value;
		bool result = NameToGuid.TryGetValue(text, out value) || NameToGuid.TryGetValue("item_" + text, out value);
		prefab = value.Item2;
		return result;
	}
}
