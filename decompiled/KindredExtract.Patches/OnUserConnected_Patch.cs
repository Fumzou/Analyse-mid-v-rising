using System;
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

[HarmonyPatch(typeof(ServerBootstrapSystem), "OnUserConnected")]
public static class OnUserConnected_Patch
{
	public unsafe static void Postfix(ServerBootstrapSystem __instance, NetConnectionId netConnectionId)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		if (Core.Players == null)
		{
			Core.InitializeAfterLoaded();
		}
		bool flag = default(bool);
		try
		{
			_ = ((ComponentSystemBase)__instance).EntityManager;
			int num = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
			Entity userEntity = ((Il2CppArrayBase<ServerClient>)(object)__instance._ApprovedUsersLookup)[num].UserEntity;
			EntityManager entityManager = ((ComponentSystemBase)__instance).EntityManager;
			User componentData = ((EntityManager)(ref entityManager)).GetComponentData<User>(userEntity);
			if (!((FixedString64Bytes)(ref componentData.CharacterName)).IsEmpty)
			{
				string text = ((object)(*(FixedString64Bytes*)(&componentData.CharacterName))/*cast due to .constrained prefix*/).ToString();
				Core.Players.UpdatePlayerCache(userEntity, text, text);
				ManualLogSource log = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(17, 1, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Player ");
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(text);
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" connected");
				}
				log.LogInfo(val);
			}
		}
		catch (Exception ex)
		{
			ManualLogSource log2 = Core.Log;
			BepInExErrorLogInterpolatedStringHandler val2 = new BepInExErrorLogInterpolatedStringHandler(51, 5, ref flag);
			if (flag)
			{
				((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("Failure in ");
				((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>("OnUserConnected");
				((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("\nMessage: ");
				((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>(ex.Message);
				((BepInExLogInterpolatedStringHandler)val2).AppendLiteral(" Inner:");
				((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>(ex.InnerException?.Message);
				((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("\n\nStack: ");
				((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>(ex.StackTrace);
				((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("\nInner Stack: ");
				((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>(ex.InnerException?.StackTrace);
			}
			log2.LogError(val2);
		}
	}
}
