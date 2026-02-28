using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Engine.Console;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using KindredExtract.Services;
using ProjectM;
using ProjectM.Network;
using ProjectM.Physics;
using ProjectM.Scripting;
using ProjectM.UI;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using VampireCommandFramework.Breadstone;

namespace KindredExtract;

internal static class Core
{
	private class TestCommand : Object
	{
		public TestCommand(IntPtr ptr)
			: base(ptr)
		{
		}

		public TestCommand()
		{
		}

		public void Test()
		{
			Log.LogInfo((object)"Test command executed!");
		}
	}

	private class ConsoleTest : ConsoleCommand
	{
		public ConsoleTest(string command)
			: base(command, 0)
		{
			Log.LogInfo((object)"Created!");
		}

		public override void ExecuteCommand(string command, string fullCommand)
		{
			Log.LogInfo((object)"ConsoleTest command executed!");
		}
	}

	private static MonoBehaviour monoBehaviour;

	private static bool _hasInitialized = false;

	public static World TheWorld { get; } = GetWorld("Server") ?? GetWorld("Client_0") ?? throw new Exception("There is no Server world (yet). Did you install a server mod on the client?");

	public static bool IsServer => TheWorld.Name == "Server";

	public static EntityManager EntityManager { get; } = TheWorld.EntityManager;

	public static ServerScriptMapper ServerScriptMapper { get; internal set; }

	public static double ServerTime => ServerGameManager.ServerTime;

	public static ServerGameManager ServerGameManager => ServerScriptMapper.GetServerGameManager();

	public static ConsoleCommandSystem ConsoleCommandSystem => TheWorld.GetExistingSystemManaged<ConsoleCommandSystem>();

	public static RefinementstationMenuMapper RefinementstationMenuMapper => TheWorld.GetExistingSystemManaged<RefinementstationMenuMapper>();

	public static LocalizationService Localization { get; } = new LocalizationService();

	public static ManualLogSource Log { get; } = Plugin.LogInstance;

	public static PlayerService Players { get; internal set; }

	public static PrefabService Prefabs { get; internal set; }

	public static EcsSystemHierarchyService EcsSystemHierarchyService { get; } = new EcsSystemHierarchyService(Log);

	public static EcsSystemDumpService EcsSystemDumpService { get; } = new EcsSystemDumpService(EcsSystemHierarchyService, Log);

	public static void LogException(Exception e, [CallerMemberName] string caller = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		ManualLogSource log = Log;
		bool flag = default(bool);
		BepInExErrorLogInterpolatedStringHandler val = new BepInExErrorLogInterpolatedStringHandler(51, 5, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Failure in ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(caller);
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("\nMessage: ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(e.Message);
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" Inner:");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(e.InnerException?.Message);
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("\n\nStack: ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(e.StackTrace);
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("\nInner Stack: ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(e.InnerException?.StackTrace);
		}
		log.LogError(val);
	}

	internal static void InitializeAfterLoaded()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		if (!_hasInitialized)
		{
			ServerScriptMapper = TheWorld.GetExistingSystemManaged<ServerScriptMapper>();
			Players = new PlayerService();
			Prefabs = new PrefabService();
			ComponentInitializer.InitializeComponents();
			_hasInitialized = true;
			ManualLogSource log = Log;
			bool flag = default(bool);
			BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(10, 1, ref flag);
			if (flag)
			{
				((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>("InitializeAfterLoaded");
				((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" completed");
			}
			log.LogInfo(val);
		}
	}

	public static (PrefabGUID, int)[] CountPrefabs(int maxNum = 10, string filter = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<PrefabGUID, int> dictionary = new Dictionary<PrefabGUID, int>();
		EntityManager entityManager = EntityManager;
		Enumerator<Entity> enumerator = ((EntityManager)(ref entityManager)).GetAllEntities((Allocator)2).GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			entityManager = EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<PrefabGUID>(current))
			{
				continue;
			}
			entityManager = EntityManager;
			PrefabGUID componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabGUID>(current);
			if (filter == null || componentData.LookupName().Contains(filter))
			{
				if (!dictionary.ContainsKey(componentData))
				{
					dictionary.Add(componentData, 0);
				}
				dictionary[componentData]++;
			}
		}
		return (from x in dictionary.OrderByDescending((KeyValuePair<PrefabGUID, int> x) => x.Value).Take(maxNum)
			select (Key: x.Key, Value: x.Value)).ToArray();
	}

	public static void SavePrefabCountToCSV()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Dictionary<PrefabGUID, int> dictionary2 = new Dictionary<PrefabGUID, int>();
		int num = 0;
		int num2 = 0;
		EntityManager entityManager = EntityManager;
		Enumerator<Entity> enumerator = ((EntityManager)(ref entityManager)).GetAllEntities((Allocator)2).GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			entityManager = EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<PrefabGUID>(current))
			{
				string componentString = current.GetComponentString();
				if (!dictionary.ContainsKey(componentString))
				{
					dictionary.Add(componentString, 0);
				}
				dictionary[componentString]++;
				num2++;
				if (componentString == "Simulate")
				{
					entityManager = EntityManager;
					((EntityManager)(ref entityManager)).DestroyEntity(current);
				}
			}
			else
			{
				entityManager = EntityManager;
				PrefabGUID componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabGUID>(current);
				if (!dictionary2.ContainsKey(componentData))
				{
					dictionary2.Add(componentData, 0);
				}
				dictionary2[componentData]++;
				num++;
			}
		}
		ManualLogSource log = Log;
		bool flag = default(bool);
		BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(20, 1, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Total Prefab Count: ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<int>(num);
		}
		log.LogInfo(val);
		ManualLogSource log2 = Log;
		val = new BepInExInfoLogInterpolatedStringHandler(24, 1, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Total Non Prefab Count: ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<int>(num2);
		}
		log2.LogInfo(val);
		List<KeyValuePair<PrefabGUID, int>> list = dictionary2.ToList();
		list.Sort((KeyValuePair<PrefabGUID, int> x, KeyValuePair<PrefabGUID, int> y) => x.Key.LookupName().CompareTo(y.Key.LookupName()));
		using (StreamWriter streamWriter = new StreamWriter("prefab_count.csv"))
		{
			streamWriter.WriteLine("PrefabGUID,Count");
			foreach (KeyValuePair<PrefabGUID, int> item in list)
			{
				streamWriter.WriteLine($"{item.Key.LookupName()},{item.Value}");
			}
		}
		List<KeyValuePair<string, int>> list2 = dictionary.ToList();
		list2.Sort((KeyValuePair<string, int> x, KeyValuePair<string, int> y) => x.Key.CompareTo(y.Key));
		using StreamWriter streamWriter2 = new StreamWriter("non_prefab_count.csv");
		streamWriter2.WriteLine("Components,Count");
		foreach (KeyValuePair<string, int> item2 in list2)
		{
			streamWriter2.WriteLine($"{item2.Key},{item2.Value}");
		}
	}

	public static void RegisterCommandsForConsole()
	{
		Log.LogInfo((object)"Registering commands!!!");
		ConsoleCommandSystem existingSystemManaged = TheWorld.GetExistingSystemManaged<ConsoleCommandSystem>();
		Type typeFromHandle = typeof(ConsoleTest);
		if (!ClassInjector.IsTypeRegisteredInIl2Cpp(typeFromHandle))
		{
			ClassInjector.RegisterTypeInIl2Cpp(typeFromHandle);
		}
		existingSystemManaged.AddCommand((ConsoleCommand)(object)new ConsoleTest("test"));
	}

	private static VChatEvent GetVChatEvent(string message)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Helper.GetEntitiesByComponentType<User>()[0];
		Entity val2 = Helper.GetEntitiesByComponentType<PlayerCharacter>()[0];
		ConstructorInfo? constructor = typeof(VChatEvent).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[5]
		{
			typeof(Entity),
			typeof(Entity),
			typeof(string),
			typeof(ChatMessageType),
			typeof(User)
		}, null);
		ChatMessageType val3 = (ChatMessageType)4;
		User val4 = val.Read<User>();
		object obj = constructor.Invoke(new object[5] { val, val2, message, val3, val4 });
		return (VChatEvent)((obj is VChatEvent) ? obj : null);
	}

	private static World GetWorld(string name)
	{
		Enumerator<World> enumerator = World.s_AllWorlds.GetEnumerator();
		while (enumerator.MoveNext())
		{
			World current = enumerator.Current;
			if (current.Name == name)
			{
				return current;
			}
		}
		return null;
	}

	public static Coroutine StartCoroutine(IEnumerator routine)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		if ((Object)(object)monoBehaviour == (Object)null)
		{
			GameObject val = new GameObject("KindredExtract");
			monoBehaviour = (MonoBehaviour)(object)val.AddComponent<IgnorePhysicsDebugSystem>();
			Object.DontDestroyOnLoad((Object)val);
		}
		return monoBehaviour.StartCoroutine(CollectionExtensions.WrapToIl2Cpp(routine));
	}

	public static void StopCoroutine(Coroutine coroutine)
	{
		if (!((Object)(object)monoBehaviour == (Object)null))
		{
			monoBehaviour.StopCoroutine(coroutine);
		}
	}
}
