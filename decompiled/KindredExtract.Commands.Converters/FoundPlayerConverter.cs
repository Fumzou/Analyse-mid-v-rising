using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using KindredExtract.Models;
using VampireCommandFramework;

namespace KindredExtract.Commands.Converters;

internal class FoundPlayerConverter : CommandArgumentConverter<FoundPlayer>
{
	public override FoundPlayer Parse(ICommandContext ctx, string input)
	{
		return new FoundPlayer(HandleFindPlayerData(ctx, input, requireOnline: false));
	}

	public static PlayerData HandleFindPlayerData(ICommandContext ctx, string input, bool requireOnline)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		ManualLogSource log = Core.Log;
		bool flag = default(bool);
		BepInExDebugLogInterpolatedStringHandler val = new BepInExDebugLogInterpolatedStringHandler(28, 1, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("FoundPlayerConverter.Parse(");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(input);
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral(")");
		}
		log.LogDebug(val);
		ulong result;
		bool flag2 = ulong.TryParse(input, out result);
		ManualLogSource log2 = Core.Log;
		val = new BepInExDebugLogInterpolatedStringHandler(13, 2, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("\tisSteam64: ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<bool>(flag2);
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<ulong>(result);
		}
		log2.LogDebug(val);
		if (flag2 && Core.Players.TryFindSteam(result, out var playerData) && (!requireOnline || playerData.IsOnline))
		{
			ManualLogSource log3 = Core.Log;
			val = new BepInExDebugLogInterpolatedStringHandler(19, 1, ref flag);
			if (flag)
			{
				((BepInExLogInterpolatedStringHandler)val).AppendLiteral("\tFound by steamid: ");
				((BepInExLogInterpolatedStringHandler)val).AppendFormatted<PlayerData>(playerData);
			}
			log3.LogDebug(val);
			return playerData;
		}
		ManualLogSource log4 = Core.Log;
		val = new BepInExDebugLogInterpolatedStringHandler(35, 0, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("\tNot found by steamid, trying name.");
		}
		log4.LogDebug(val);
		if (Core.Players.TryFindName(input.ToLower(), out var playerData2) && (!requireOnline || playerData2.IsOnline))
		{
			ManualLogSource log5 = Core.Log;
			val = new BepInExDebugLogInterpolatedStringHandler(16, 1, ref flag);
			if (flag)
			{
				((BepInExLogInterpolatedStringHandler)val).AppendLiteral("\tFound by name: ");
				((BepInExLogInterpolatedStringHandler)val).AppendFormatted<PlayerData>(playerData2);
			}
			log5.LogDebug(val);
			return playerData2;
		}
		ManualLogSource log6 = Core.Log;
		val = new BepInExDebugLogInterpolatedStringHandler(35, 0, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("\tNot found by name, throwing error.");
		}
		log6.LogDebug(val);
		throw ctx.Error("Player " + input + " not found.");
	}
}
