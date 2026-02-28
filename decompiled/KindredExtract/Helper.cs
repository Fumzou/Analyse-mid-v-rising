using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using ProjectM;
using ProjectM.Shared;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;

namespace KindredExtract;

internal static class Helper
{
	public static AdminAuthSystem adminAuthSystem = Core.TheWorld.GetExistingSystemManaged<AdminAuthSystem>();

	public static PrefabGUID GetPrefabGUID(Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = Core.EntityManager;
		PrefabGUID componentData = default(PrefabGUID);
		try
		{
			componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabGUID>(entity);
			return componentData;
		}
		catch
		{
			((PrefabGUID)(ref componentData))._002Ector(0);
		}
		return componentData;
	}

	public static Entity AddItemToInventory(Entity recipient, PrefabGUID guid, int amount)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			GameDataSystem existingSystemManaged = Core.TheWorld.GetExistingSystemManaged<GameDataSystem>();
			return InventoryUtilitiesServer.TryAddItem(AddItemSettings.Create(Core.EntityManager, existingSystemManaged.ItemHashLookupMap, false, default(Entity), default(Nullable_Unboxed<int>), false, false, false, default(Nullable_Unboxed<int>)), recipient, guid, amount).NewEntity;
		}
		catch (Exception e)
		{
			Core.LogException(e, "AddItemToInventory");
		}
		return default(Entity);
	}

	public static NativeArray<Entity> GetEntitiesByComponentType<T1>(bool includeAll = false, bool includeDisabled = false, bool includeSpawn = false, bool includePrefab = false, bool includeDestroyed = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryOptions val = (EntityQueryOptions)0;
		if (includeAll)
		{
			val = (EntityQueryOptions)(val | 0xC3);
		}
		if (includeDisabled)
		{
			val = (EntityQueryOptions)(val | 2);
		}
		if (includeSpawn)
		{
			val = (EntityQueryOptions)(val | 0x40);
		}
		if (includePrefab)
		{
			val = (EntityQueryOptions)(val | 1);
		}
		if (includeDestroyed)
		{
			val = (EntityQueryOptions)(val | 0x80);
		}
		EntityQueryBuilder val2 = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val2))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val2)).AddAll(new ComponentType(Il2CppType.Of<T1>(), (AccessMode)0));
		((EntityQueryBuilder)(ref val2)).WithOptions(val);
		EntityManager entityManager = Core.EntityManager;
		EntityQuery val3 = ((EntityManager)(ref entityManager)).CreateEntityQuery(ref val2);
		return ((EntityQuery)(ref val3)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
	}

	public static NativeArray<Entity> GetEntitiesByComponentTypes<T1, T2>(bool includeAll = false, bool includeDisabled = false, bool includeSpawn = false, bool includePrefab = false, bool includeDestroyed = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryOptions val = (EntityQueryOptions)0;
		if (includeAll)
		{
			val = (EntityQueryOptions)(val | 0xC3);
		}
		if (includeDisabled)
		{
			val = (EntityQueryOptions)(val | 2);
		}
		if (includeSpawn)
		{
			val = (EntityQueryOptions)(val | 0x40);
		}
		if (includePrefab)
		{
			val = (EntityQueryOptions)(val | 1);
		}
		if (includeDestroyed)
		{
			val = (EntityQueryOptions)(val | 0x80);
		}
		EntityQueryBuilder val2 = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val2))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val2)).AddAll(new ComponentType(Il2CppType.Of<T1>(), (AccessMode)0));
		((EntityQueryBuilder)(ref val2)).AddAll(new ComponentType(Il2CppType.Of<T2>(), (AccessMode)0));
		((EntityQueryBuilder)(ref val2)).WithOptions(val);
		EntityManager entityManager = Core.EntityManager;
		EntityQuery val3 = ((EntityManager)(ref entityManager)).CreateEntityQuery(ref val2);
		return ((EntityQuery)(ref val3)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
	}

	public static void RepairGear(Entity Character, bool repair = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		Equipment val = Character.Read<Equipment>();
		NativeList<Entity> val2 = default(NativeList<Entity>);
		val2._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((Equipment)(ref val)).GetAllEquipmentEntities(val2, false);
		Enumerator<Entity> enumerator = val2.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			if (current.Has<Durability>())
			{
				Durability val3 = current.Read<Durability>();
				if (repair)
				{
					val3.Value = val3.MaxDurability;
				}
				else
				{
					val3.Value = 0f;
				}
				current.Write<Durability>(val3);
			}
		}
		val2.Dispose();
		InventoryBuffer val4 = default(InventoryBuffer);
		for (int i = 0; i < 36; i++)
		{
			if (!InventoryUtilities.TryGetItemAtSlot<EntityManager>(Core.EntityManager, Character, i, ref val4))
			{
				continue;
			}
			Entity entity = val4.ItemEntity._Entity;
			if (entity.Has<Durability>())
			{
				Durability val5 = entity.Read<Durability>();
				if (repair)
				{
					val5.Value = val5.MaxDurability;
				}
				else
				{
					val5.Value = 0f;
				}
				entity.Write<Durability>(val5);
			}
		}
	}

	public unsafe static bool FindClan(string clanName, out Entity clanEntity)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity item in ((IEnumerable<Entity>)GetEntitiesByComponentType<ClanTeam>().ToArray()).Where(delegate(Entity x)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			ClanTeam val = x.Read<ClanTeam>();
			return ((object)(*(FixedString64Bytes*)(&val.Name))/*cast due to .constrained prefix*/).ToString().ToLower() == clanName.ToLower();
		}))
		{
			EntityManager entityManager = Core.EntityManager;
			if (((EntityManager)(ref entityManager)).GetBuffer<ClanMemberStatus>(item, false).Length != 0)
			{
				clanEntity = item;
				return true;
			}
		}
		clanEntity = default(Entity);
		return false;
	}
}
