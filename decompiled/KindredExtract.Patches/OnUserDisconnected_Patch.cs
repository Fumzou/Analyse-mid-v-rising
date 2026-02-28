using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using ProjectM;
using ProjectM.Network;
using Stunlock.Network;
using Unity.Collections;
using Unity.Entities;

namespace KindredExtract.Patches;

[HarmonyPatch(typeof(ServerBootstrapSystem), "OnUserDisconnected")]
public static class OnUserDisconnected_Patch
{
	private unsafe static void Prefix(ServerBootstrapSystem __instance, NetConnectionId netConnectionId, ConnectionStatusChangeReason connectionStatusReason, string extraData)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		if (Core.Players == null)
		{
			Core.InitializeAfterLoaded();
		}
		try
		{
			int num = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
			ServerClient val = ((Il2CppArrayBase<ServerClient>)(object)__instance._ApprovedUsersLookup)[num];
			EntityManager entityManager = ((ComponentSystemBase)__instance).EntityManager;
			User componentData = ((EntityManager)(ref entityManager)).GetComponentData<User>(val.UserEntity);
			if (!((FixedString64Bytes)(ref componentData.CharacterName)).IsEmpty)
			{
				string text = ((object)(*(FixedString64Bytes*)(&componentData.CharacterName))/*cast due to .constrained prefix*/).ToString();
				Core.Players.UpdatePlayerCache(val.UserEntity, text, text, forceOffline: true);
				ManualLogSource log = Core.Log;
				bool flag = default(bool);
				BepInExInfoLogInterpolatedStringHandler val2 = new BepInExInfoLogInterpolatedStringHandler(20, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("Player ");
					((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>(text);
					((BepInExLogInterpolatedStringHandler)val2).AppendLiteral(" disconnected");
				}
				log.LogInfo(val2);
			}
		}
		catch
		{
		}
	}
}
