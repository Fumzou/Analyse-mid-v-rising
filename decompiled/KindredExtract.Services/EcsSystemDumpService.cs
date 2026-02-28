using System.IO;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using Il2CppSystem.Collections.Generic;
using KindredExtract.Models;
using Unity.Entities;

namespace KindredExtract.Services;

public class EcsSystemDumpService
{
	private ManualLogSource Log;

	private EcsSystemHierarchyService EcsSystemHierarchyService;

	public EcsSystemDumpService(EcsSystemHierarchyService ecsSystemHierarchyService, ManualLogSource log)
	{
		Log = log;
		EcsSystemHierarchyService = ecsSystemHierarchyService;
	}

	public string DumpSystemsUpdateTrees()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		string text = "Dump/Systems/UpdateTree/";
		Directory.CreateDirectory(text);
		EcsSystemDumper ecsSystemDumper = new EcsSystemDumper();
		Enumerator<World> enumerator = World.s_AllWorlds.GetEnumerator();
		while (enumerator.MoveNext())
		{
			World current = enumerator.Current;
			EcsSystemHierarchy systemHierarchy = EcsSystemHierarchyService.BuildSystemHiearchyForWorld(current);
			File.WriteAllText(text + "/" + current.Name + ".txt", ecsSystemDumper.CreateDumpString(systemHierarchy));
		}
		ManualLogSource log = Log;
		bool flag = default(bool);
		BepInExMessageLogInterpolatedStringHandler val = new BepInExMessageLogInterpolatedStringHandler(40, 1, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Dumped system hierarchy files to folder ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(text);
		}
		log.LogMessage(val);
		return text;
	}
}
