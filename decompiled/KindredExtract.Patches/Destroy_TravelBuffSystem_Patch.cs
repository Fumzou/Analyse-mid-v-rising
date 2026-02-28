using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using HarmonyLib;
using KindredExtract.Data;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;

namespace KindredExtract.Patches;

[HarmonyPatch(typeof(Destroy_TravelBuffSystem), "OnUpdate")]
public class Destroy_TravelBuffSystem_Patch
{
	private unsafe static void Postfix(Destroy_TravelBuffSystem __instance)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		if (Core.Players == null)
		{
			Core.InitializeAfterLoaded();
		}
		EntityQuery _query_615927226_ = __instance.__query_615927226_0;
		Enumerator<Entity> enumerator = ((EntityQuery)(ref _query_615927226_)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
		bool flag = default(bool);
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			EntityManager entityManager = ((ComponentSystemBase)__instance).EntityManager;
			PrefabGUID componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabGUID>(current);
			if (((PrefabGUID)(ref componentData)).Equals(Prefabs.AB_Interact_TombCoffinSpawn_Travel))
			{
				entityManager = ((ComponentSystemBase)__instance).EntityManager;
				Entity owner = ((EntityManager)(ref entityManager)).GetComponentData<EntityOwner>(current).Owner;
				entityManager = ((ComponentSystemBase)__instance).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<PlayerCharacter>(owner))
				{
					break;
				}
				entityManager = ((ComponentSystemBase)__instance).EntityManager;
				Entity userEntity = ((EntityManager)(ref entityManager)).GetComponentData<PlayerCharacter>(owner).UserEntity;
				entityManager = ((ComponentSystemBase)__instance).EntityManager;
				User componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<User>(userEntity);
				string text = ((object)(*(FixedString64Bytes*)(&componentData2.CharacterName))/*cast due to .constrained prefix*/).ToString();
				Core.Players.UpdatePlayerCache(userEntity, text, text);
				ManualLogSource log = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(15, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Player ");
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(text);
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" created");
				}
				log.LogInfo(val);
			}
		}
	}
}
