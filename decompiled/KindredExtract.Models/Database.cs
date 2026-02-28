using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace KindredExtract.Models;

public readonly struct Database
{
	private readonly ConfigFile CONFIG;

	private static readonly string CONFIG_PATH = Path.Combine(Paths.ConfigPath, "KindredExtract");

	private static readonly string STAFF_PATH = Path.Combine(CONFIG_PATH, "staff.json");

	private static readonly string NOSPAWN_PATH = Path.Combine(CONFIG_PATH, "nospawn.json");

	private static readonly Dictionary<string, string> STAFF = new Dictionary<string, string>
	{
		{ "SteamID1", "[Rank]" },
		{ "SteamID2", "[Rank]" }
	};

	private static readonly Dictionary<string, string> NOSPAWN = new Dictionary<string, string> { { "PrefabGUID", "Reason" } };

	public Database(ConfigFile config)
	{
		CONFIG = config;
	}

	public void InitConfig()
	{
		STAFF.Clear();
		NOSPAWN.Clear();
		if (File.Exists(STAFF_PATH))
		{
			foreach (KeyValuePair<string, string> item in JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(STAFF_PATH)))
			{
				STAFF.Add(item.Key, item.Value);
			}
		}
		if (File.Exists(NOSPAWN_PATH))
		{
			foreach (KeyValuePair<string, string> item2 in JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(NOSPAWN_PATH)))
			{
				NOSPAWN.Add(item2.Key, item2.Value);
			}
			return;
		}
		NOSPAWN["CHAR_VampireMale"] = "it causes corruption to the save file.";
		NOSPAWN["CHAR_Mount_Horse_Gloomrot"] = "it causes an instant server crash.";
		NOSPAWN["CHAR_Mount_Horse_Vampire"] = "it causes an instant server crash.";
		SaveNoSpawn();
	}

	private static void WriteConfig(string path, Dictionary<string, string> dic)
	{
		if (!Directory.Exists(CONFIG_PATH))
		{
			Directory.CreateDirectory(CONFIG_PATH);
		}
		if (!File.Exists(path))
		{
			string contents = JsonSerializer.Serialize(dic, new JsonSerializerOptions
			{
				WriteIndented = true
			});
			File.WriteAllText(path, contents);
		}
	}

	public static void SaveStaff()
	{
		WriteConfig(STAFF_PATH, STAFF);
	}

	public static void SaveNoSpawn()
	{
		WriteConfig(NOSPAWN_PATH, NOSPAWN);
	}

	public static void SetStaff(Entity userEntity, string rank)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		User val = userEntity.Read<User>();
		STAFF[val.PlatformId.ToString()] = rank;
		SaveStaff();
		ManualLogSource log = Core.Log;
		bool flag = default(bool);
		BepInExWarningLogInterpolatedStringHandler val2 = new BepInExWarningLogInterpolatedStringHandler(32, 2, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("User ");
			((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<FixedString64Bytes>(val.CharacterName);
			((BepInExLogInterpolatedStringHandler)val2).AppendLiteral(" added to staff config as ");
			((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<string>(rank);
			((BepInExLogInterpolatedStringHandler)val2).AppendLiteral(".");
		}
		log.LogWarning(val2);
	}

	public static void SetNoSpawn(string prefabName, string reason)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		NOSPAWN[prefabName] = reason;
		SaveNoSpawn();
		ManualLogSource log = Core.Log;
		bool flag = default(bool);
		BepInExWarningLogInterpolatedStringHandler val = new BepInExWarningLogInterpolatedStringHandler(38, 2, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral("NPC ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(prefabName);
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" is banned from spawning because ");
			((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(reason);
			((BepInExLogInterpolatedStringHandler)val).AppendLiteral(".");
		}
		log.LogWarning(val);
	}

	public static string GetStaff(Entity user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Player player = new Player(user);
		if (player != null && STAFF.ContainsKey(player.SteamID.ToString()))
		{
			return STAFF[player.SteamID.ToString()];
		}
		return null;
	}

	public static bool IsSpawnBanned(string prefabName, out string reason)
	{
		return NOSPAWN.TryGetValue(prefabName, out reason);
	}

	public static Dictionary<string, string> GetStaff()
	{
		return STAFF;
	}

	public static Dictionary<string, string> GetNoSpawn()
	{
		return NOSPAWN;
	}

	public bool RemoveStaff(Entity userEntity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		bool num = STAFF.Remove(userEntity.Read<User>().PlatformId.ToString());
		bool flag = default(bool);
		if (num)
		{
			SaveStaff();
			ManualLogSource log = Core.Log;
			BepInExWarningLogInterpolatedStringHandler val = new BepInExWarningLogInterpolatedStringHandler(32, 1, ref flag);
			if (flag)
			{
				((BepInExLogInterpolatedStringHandler)val).AppendLiteral("User ");
				((BepInExLogInterpolatedStringHandler)val).AppendFormatted<FixedString64Bytes>(userEntity.Read<User>().CharacterName);
				((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" removed from staff config.");
			}
			log.LogWarning(val);
			return num;
		}
		ManualLogSource log2 = Core.Log;
		BepInExInfoLogInterpolatedStringHandler val2 = new BepInExInfoLogInterpolatedStringHandler(65, 1, ref flag);
		if (flag)
		{
			((BepInExLogInterpolatedStringHandler)val2).AppendLiteral("User ");
			((BepInExLogInterpolatedStringHandler)val2).AppendFormatted<FixedString64Bytes>(userEntity.Read<User>().CharacterName);
			((BepInExLogInterpolatedStringHandler)val2).AppendLiteral(" attempted to be removed from staff config but wasn't there.");
		}
		log2.LogInfo(val2);
		return num;
	}
}
