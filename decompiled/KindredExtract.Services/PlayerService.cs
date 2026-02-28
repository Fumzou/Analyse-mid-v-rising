using System.Collections.Generic;
using System.Linq;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using KindredExtract.Models;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace KindredExtract.Services;

internal class PlayerService
{
	private Dictionary<string, PlayerData> NamePlayerCache = new Dictionary<string, PlayerData>();

	private Dictionary<ulong, PlayerData> SteamPlayerCache = new Dictionary<ulong, PlayerData>();

	internal bool TryFindSteam(ulong steamId, out PlayerData playerData)
	{
		return SteamPlayerCache.TryGetValue(steamId, out playerData);
	}

	internal bool TryFindName(string name, out PlayerData playerData)
	{
		return NamePlayerCache.TryGetValue(name, out playerData);
	}

	internal unsafe PlayerService()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		NamePlayerCache.Clear();
		SteamPlayerCache.Clear();
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).AddAll(ComponentType.ReadOnly<User>());
		((EntityQueryBuilder)(ref val)).WithOptions((EntityQueryOptions)2);
		EntityManager entityManager = Core.EntityManager;
		EntityQuery val2 = ((EntityManager)(ref entityManager)).CreateEntityQuery(ref val);
		Enumerator<Entity> enumerator = ((EntityQuery)(ref val2)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			entityManager = Core.EntityManager;
			User componentData = ((EntityManager)(ref entityManager)).GetComponentData<User>(current);
			PlayerData value = new PlayerData(((object)(*(FixedString64Bytes*)(&componentData.CharacterName))/*cast due to .constrained prefix*/).ToString(), componentData.PlatformId, componentData.IsConnected, current, componentData.LocalCharacter._Entity);
			NamePlayerCache.TryAdd(((object)(*(FixedString64Bytes*)(&componentData.CharacterName))/*cast due to .constrained prefix*/).ToString().ToLower(), value);
			SteamPlayerCache.TryAdd(componentData.PlatformId, value);
		}
		IEnumerable<string> enumerable = from p in NamePlayerCache.Values
			where p.IsOnline
			select "\t" + p.CharacterName;
		ManualLogSource log = Core.Log;
		bool flag = default(bool);
		BepInExWarningLogInterpolatedStringHandler val3 = new BepInExWarningLogInterpolatedStringHandler(58, 2, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val3).AppendLiteral("Player Cache Created with ");
			((BepInExLogInterpolatedStringHandler)val3).AppendFormatted<int>(NamePlayerCache.Count);
			((BepInExLogInterpolatedStringHandler)val3).AppendLiteral(" entries total, listing ");
			((BepInExLogInterpolatedStringHandler)val3).AppendFormatted<int>(enumerable.Count());
			((BepInExLogInterpolatedStringHandler)val3).AppendLiteral(" online:");
		}
		log.LogWarning(val3);
		Core.Log.LogWarning((object)string.Join("\n", enumerable));
	}

	internal void UpdatePlayerCache(Entity userEntity, string oldName, string newName, bool forceOffline = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = Core.EntityManager;
		User componentData = ((EntityManager)(ref entityManager)).GetComponentData<User>(userEntity);
		NamePlayerCache.Remove(oldName.ToLower());
		if (forceOffline)
		{
			componentData.IsConnected = false;
		}
		PlayerData value = new PlayerData(newName, componentData.PlatformId, componentData.IsConnected, userEntity, componentData.LocalCharacter._Entity);
		NamePlayerCache[newName.ToLower()] = value;
		SteamPlayerCache[componentData.PlatformId] = value;
	}

	internal unsafe bool RenamePlayer(Entity userEntity, Entity charEntity, string newName)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		DebugEventsSystem existingSystemManaged = Core.TheWorld.GetExistingSystemManaged<DebugEventsSystem>();
		EntityManager entityManager = Core.EntityManager;
		NetworkId componentData = ((EntityManager)(ref entityManager)).GetComponentData<NetworkId>(userEntity);
		entityManager = Core.EntityManager;
		User componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<User>(userEntity);
		RenameUserDebugEvent val = new RenameUserDebugEvent
		{
			NewName = FixedString64Bytes.op_Implicit(newName),
			Target = componentData
		};
		FromCharacter val2 = new FromCharacter
		{
			User = userEntity,
			Character = charEntity
		};
		existingSystemManaged.RenameUser(val2, val);
		UpdatePlayerCache(userEntity, ((object)(*(FixedString64Bytes*)(&componentData2.CharacterName))/*cast due to .constrained prefix*/).ToString(), newName.ToString());
		return true;
	}

	public static IEnumerable<Entity> GetUsersOnline()
	{
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).AddAll(ComponentType.ReadOnly<User>());
		EntityManager entityManager = Core.TheWorld.EntityManager;
		EntityQuery val2 = ((EntityManager)(ref entityManager)).CreateEntityQuery(ref val);
		NativeArray<Entity> _userEntities = ((EntityQuery)(ref val2)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		int len = _userEntities.Length;
		int i = 0;
		while (i < len)
		{
			if (_userEntities[i].Read<User>().IsConnected)
			{
				yield return _userEntities[i];
			}
			int num = i + 1;
			i = num;
		}
	}
}
