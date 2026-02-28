using System.Reflection;
using BepInEx;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using KindredExtract.Models;
using ProjectM;
using VampireCommandFramework;

namespace KindredExtract;

[BepInPlugin("KindredExtract", "KindredExtract", "1.8.2")]
[BepInDependency(/*Could not decode attribute arguments.*/)]
public class Plugin : BasePlugin
{
	private static Harmony _harmony;

	public static Harmony Harmony => _harmony;

	public static ManualLogSource LogInstance { get; private set; }

	public static Database Settings { get; private set; }

	public override void Load()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		ManualLogSource log = ((BasePlugin)this).Log;
		bool flag = default(bool);
		BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(27, 2, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Plugin ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>("KindredExtract");
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" version ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>("1.8.2");
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" is loaded!");
		}
		log.LogInfo(val);
		LogInstance = ((BasePlugin)this).Log;
		Settings = new Database(((BasePlugin)this).Config);
		Settings.InitConfig();
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		_harmony = new Harmony("KindredExtract");
		((BasePlugin)this).Log.LogInfo((object)"Harmony Patching");
		_harmony.PatchAll(executingAssembly);
		((BasePlugin)this).Log.LogInfo((object)"Registering commands");
		CommandRegistry.RegisterAll(executingAssembly);
	}

	public override bool Unload()
	{
		CommandRegistry.UnregisterAssembly();
		Harmony harmony = _harmony;
		if (harmony != null)
		{
			harmony.UnpatchSelf();
		}
		return true;
	}

	public void OnGameInitialized()
	{
		if (!HasLoaded())
		{
			((BasePlugin)this).Log.LogDebug((object)"Attempt to initialize before everything has loaded.");
		}
		else
		{
			Core.InitializeAfterLoaded();
		}
	}

	private static bool HasLoaded()
	{
		PrefabCollectionSystem existingSystemManaged = Core.TheWorld.GetExistingSystemManaged<PrefabCollectionSystem>();
		if (existingSystemManaged == null)
		{
			return false;
		}
		return existingSystemManaged.SpawnableNameToPrefabGuidDictionary.Count > 0;
	}
}
