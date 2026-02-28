using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VampireCommandFramework;

namespace KindredExtract.Commands;

[CommandGroup("entity", "e")]
internal static class EntityCommands
{
	[Command("teleport", "tp", null, "Teleport to the specified entity.", null, true)]
	public static void TeleportToEntity(ChatCommandContext ctx, int entityId, int version = 1)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		_ = ctx.Event.SenderCharacterEntity;
		Entity val = new Entity
		{
			Index = entityId,
			Version = version
		};
		EntityManager entityManager = Core.EntityManager;
		if (!((EntityManager)(ref entityManager)).Exists(val))
		{
			ctx.Reply("Specified entity doesn't exist");
			return;
		}
		float3 value = val.Read<Translation>().Value;
		entityManager = Core.EntityManager;
		EntityManager entityManager2 = Core.EntityManager;
		Entity val2 = ((EntityManager)(ref entityManager)).CreateEntity(((EntityManager)(ref entityManager2)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<FromCharacter>(),
			ComponentType.ReadWrite<PlayerTeleportDebugEvent>()
		}));
		entityManager = Core.EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<FromCharacter>(val2, new FromCharacter
		{
			User = ctx.Event.SenderUserEntity,
			Character = ctx.Event.SenderCharacterEntity
		});
		entityManager = Core.EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<PlayerTeleportDebugEvent>(val2, new PlayerTeleportDebugEvent
		{
			Position = new float3(value.x, value.y, value.z),
			Target = (TeleportTarget)0
		});
		string value2 = $"Entity({entityId}:{version})";
		if (val.Has<PrefabGUID>())
		{
			value2 = val.Read<PrefabGUID>().LookupName();
		}
		ctx.Reply($"Teleported to {value2} at {value}");
	}

	[Command("despawn", "d", null, "Despawn the specified entity.", null, true)]
	public static void DespawnEntity(ChatCommandContext ctx, int entityId, int version = 1)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		Entity val = new Entity
		{
			Index = entityId,
			Version = version
		};
		EntityManager entityManager = Core.EntityManager;
		if (!((EntityManager)(ref entityManager)).Exists(val))
		{
			ctx.Reply("Specified entity doesn't exist");
			return;
		}
		string text = $"Entity({entityId}:{version})";
		if (val.Has<PrefabGUID>())
		{
			text = val.Read<PrefabGUID>().LookupName();
		}
		StatChangeUtility.KillEntity(Core.EntityManager, val, ctx.Event.SenderCharacterEntity, (double)Time.time, (StatChangeReason)1, true);
		ctx.Reply("Despawned " + text);
	}

	[Command("destroy", "del", null, "Destroy the specified entity.", null, true)]
	public static void DestroyEntity(ChatCommandContext ctx, int entityId, int version = 1)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Entity val = new Entity
		{
			Index = entityId,
			Version = version
		};
		EntityManager entityManager = Core.EntityManager;
		if (!((EntityManager)(ref entityManager)).Exists(val))
		{
			ctx.Reply("Specified entity doesn't exist");
			return;
		}
		string text = $"Entity({entityId}:{version})";
		if (val.Has<PrefabGUID>())
		{
			text = val.Read<PrefabGUID>().LookupName();
		}
		val.Add<DestroyTag>();
		if (!val.Has<DestroyData>())
		{
			val.Add<DestroyData>();
		}
		val.Write<DestroyData>(new DestroyData
		{
			DestroyReason = (DestroyReason)0
		});
		ctx.Reply("Destroyed " + text);
	}

	[Command("topcount", "tc", null, "Counts the top entities in the world", null, true)]
	public static void TopEntityCount(ChatCommandContext ctx, int topNum = 10, string filter = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		(PrefabGUID, int)[] array = Core.CountPrefabs(topNum, filter);
		for (int i = 0; i < array.Length; i++)
		{
			var (prefabGuid, value) = array[i];
			ctx.Reply($"{prefabGuid.LookupName()} - {value}");
		}
	}
}
