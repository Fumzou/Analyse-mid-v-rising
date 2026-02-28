using System.Reflection;
using HarmonyLib;
using Unity.Scenes;

namespace KindredExtract.Patches;

[HarmonyPatch(typeof(SceneSectionStreamingSystem), "ShutdownAsynchrnonousStreamingSupport")]
public static class InitializationPatch
{
	[HarmonyPostfix]
	public static void OneShot_AfterLoad_InitializationPatch()
	{
		Core.InitializeAfterLoaded();
		Plugin.Harmony.Unpatch((MethodBase)typeof(SceneSectionStreamingSystem).GetMethod("ShutdownAsynchrnonousStreamingSupport"), typeof(InitializationPatch).GetMethod("OneShot_AfterLoad_InitializationPatch"));
	}
}
