using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Il2CppSystem;
using Il2CppSystem.Text;
using KindredExtract.Commands.Converters;
using KindredExtract.Data;
using ProjectM;
using ProjectM.CastleBuilding;
using ProjectM.Network;
using ProjectM.Physics;
using ProjectM.Terrain;
using ProjectM.Tiles;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VampireCommandFramework;

namespace KindredExtract.Commands;

[CommandGroup("state", "s")]
public class StateCommands
{
	private struct PasteBinKeys
	{
		public string ApiKey;

		public string UserKey;

		public string FolderKey;
	}

	private static readonly string StateFolder = Path.Combine(Directory.GetCurrentDirectory(), "EntityStateFiles");

	private static readonly string ItemDataFolder = Path.Combine(Directory.GetCurrentDirectory(), "ItemData");

	private static Dictionary<ulong, PasteBinKeys> pasteBinKeys = new Dictionary<ulong, PasteBinKeys>();

	private static bool useProjectMDump;

	[Command("switchdump", null, null, "Switches between Kindred and ProjectM entity dumping", null, true)]
	public static void SwitchDump(ChatCommandContext ctx)
	{
		useProjectMDump = !useProjectMDump;
		if (useProjectMDump)
		{
			ctx.Reply("Swapped to ProjectM entity dumping");
		}
		else
		{
			ctx.Reply("Swapped to Kindred entity dumping");
		}
	}

	private static void CopyStateFileToPrev(string fileName)
	{
		if (File.Exists(Path.Combine(StateFolder, fileName)))
		{
			string text = Path.Combine(StateFolder, Path.GetFileNameWithoutExtension(fileName) + "_Prev" + Path.GetExtension(fileName));
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			File.Copy(Path.Combine(StateFolder, fileName), text);
		}
	}

	public unsafe static string OutputEntityState(ChatCommandContext ctx = null, Entity entity = default(Entity), string fileName = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1104: Unknown result type (might be due to invalid IL or missing references)
		//IL_110b: Expected O, but got Unknown
		//IL_1110: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1121: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_112e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1133: Unknown result type (might be due to invalid IL or missing references)
		//IL_1137: Unknown result type (might be due to invalid IL or missing references)
		//IL_1139: Unknown result type (might be due to invalid IL or missing references)
		//IL_113e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_119c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_1149: Unknown result type (might be due to invalid IL or missing references)
		//IL_114e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1157: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_116d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1172: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1002: Unknown result type (might be due to invalid IL or missing references)
		//IL_101c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1036: Unknown result type (might be due to invalid IL or missing references)
		//IL_104f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d33: Unknown result type (might be due to invalid IL or missing references)
		//IL_106d: Unknown result type (might be due to invalid IL or missing references)
		//IL_106e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1073: Unknown result type (might be due to invalid IL or missing references)
		//IL_108d: Unknown result type (might be due to invalid IL or missing references)
		//IL_108f: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c0: Unknown result type (might be due to invalid IL or missing references)
		Directory.CreateDirectory(StateFolder);
		if (fileName == null)
		{
			fileName = $"Entity_{entity.Index}_{entity.Version}";
			string text = "";
			string text2 = null;
			string text3 = null;
			if (entity.Has<PrefabGUID>())
			{
				text = entity.Read<PrefabGUID>().LookupName();
				fileName = $"Entity_{text}_{entity.Index}_{entity.Version}";
			}
			if (entity.Has<Attached>())
			{
				Attached val = entity.Read<Attached>();
				if (!((Entity)(ref val.Parent)).Equals(Entity.Null))
				{
					if (val.Parent.Has<PlayerCharacter>())
					{
						PlayerCharacter val2 = val.Parent.Read<PlayerCharacter>();
						text2 = ((object)(*(FixedString64Bytes*)(&val2.Name))/*cast due to .constrained prefix*/).ToString();
					}
					if (val.Parent.Has<PrefabGUID>())
					{
						text3 = val.Parent.Read<PrefabGUID>().LookupName();
					}
				}
			}
			if (entity.Has<User>())
			{
				fileName = $"User_{entity.Read<User>().CharacterName}_{entity.Index}_{entity.Version}";
			}
			else if (entity.Has<PlayerCharacter>())
			{
				fileName = $"Player_{entity.Read<PlayerCharacter>().Name}_{entity.Index}_{entity.Version}";
			}
			else if (entity.Has<CastleHeart>())
			{
				fileName = $"Castle_{entity.Index}_{entity.Version}";
			}
			else if (entity.Has<Door>())
			{
				fileName = $"Door_{entity.Index}_{entity.Version}";
			}
			else if (entity.Has<InventoryConnection>())
			{
				InventoryConnection val3 = entity.Read<InventoryConnection>();
				fileName = (((Entity)(ref val3.InventoryOwner)).Equals(Entity.Null) ? $"Inventory_{text}_{entity.Index}_{entity.Version}" : (val3.InventoryOwner.Has<PlayerCharacter>() ? $"Inventory_{val3.InventoryOwner.Read<PlayerCharacter>().Name}_{text}_{entity.Index}_{entity.Version}" : ((!val3.InventoryOwner.Has<NameableInteractable>()) ? $"Inventory_{val3.InventoryOwner.Read<PrefabGUID>().LookupName()}_{text}_{entity.Index}_{entity.Version}" : $"Inventory_{val3.InventoryOwner.Read<NameableInteractable>().Name}_{text}_{entity.Index}_{entity.Version}")));
			}
			else if (entity.Has<VBloodProgressionUnlockData>())
			{
				FixedString64Bytes characterName = entity.Read<Attach>().Parent.Read<User>().CharacterName;
				fileName = $"Progression_{characterName}_{entity.Index}_{entity.Version}";
			}
			else if (entity.Has<TeamData>())
			{
				if (entity.Has<ClanTeam>())
				{
					fileName = $"Clan_{entity.Read<ClanTeam>().Name}_{entity.Index}_{entity.Version}";
				}
				else if (entity.Has<UserTeam>())
				{
					UserTeam val4 = entity.Read<UserTeam>();
					fileName = ((!((Entity)(ref val4.UserEntity)).Equals(Entity.Null)) ? $"UserTeam_{entity.Read<UserTeam>().UserEntity.Read<User>().CharacterName}_{entity.Index}_{entity.Version}" : $"UserTeam_{entity.Index}_{entity.Version}");
				}
				else if (entity.Has<CastleTeamData>())
				{
					Entity castleHeart = entity.Read<CastleTeamData>().CastleHeart;
					if (castleHeart.Has<UserOwner>())
					{
						UserOwner val5 = castleHeart.Read<UserOwner>();
						Entity entityOnServer = ((NetworkedEntity)(ref val5.Owner)).GetEntityOnServer();
						if (!((Entity)(ref entityOnServer)).Equals(Entity.Null))
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 3);
							defaultInterpolatedStringHandler.AppendLiteral("CastleTeam_");
							val5 = castleHeart.Read<UserOwner>();
							defaultInterpolatedStringHandler.AppendFormatted<FixedString64Bytes>(((NetworkedEntity)(ref val5.Owner)).GetEntityOnServer().Read<User>().CharacterName);
							defaultInterpolatedStringHandler.AppendLiteral("_");
							defaultInterpolatedStringHandler.AppendFormatted(entity.Index);
							defaultInterpolatedStringHandler.AppendLiteral("_");
							defaultInterpolatedStringHandler.AppendFormatted(entity.Version);
							fileName = defaultInterpolatedStringHandler.ToStringAndClear();
							goto IL_10d4;
						}
					}
					fileName = $"CastleTeam_{entity.Index}";
				}
				else
				{
					fileName = $"Team_{entity.Index}_{entity.Version}";
				}
			}
			else if (entity.Has<TileModel>())
			{
				fileName = ((!entity.Has<PrefabGUID>()) ? $"TileModel_{entity.Index}_{entity.Version}" : $"TileModel_{entity.Index}_{entity.Version}_({text})");
			}
			else if (entity.Has<Buff>())
			{
				fileName = ((!(text != "")) ? $"Buff_{entity.Index}_{entity.Version}" : ((text2 != null) ? $"Buff_{entity.Index}_{entity.Version}_({text} on Player {text2})" : ((text3 == null) ? $"Buff_{entity.Index}_{entity.Version}_({text})" : $"Buff_{entity.Index}_{entity.Version}_({text} on {text3})")));
			}
			else if (entity.Has<AchievementClaimedElement>())
			{
				Entity parent = entity.Read<Attached>().Parent;
				fileName = $"Achievements_{parent.Read<User>().CharacterName}_{entity.Index}_{entity.Version}";
			}
			else if (entity.Has<UserMapZonePackedRevealElement>())
			{
				Entity parent2 = entity.Read<Attached>().Parent;
				fileName = $"UserMapZone_{parent2.Read<User>().CharacterName}_{entity.Index}_{entity.Version}";
			}
			else if (entity.Has<InventoryBuffer>())
			{
				fileName = $"Inventory_{entity.Index}_{entity.Version}";
				if (text != "")
				{
					fileName = $"Inventory_{entity.Index}_{entity.Version}_({text})";
				}
			}
			else if (entity.Has<MapIconData>())
			{
				fileName = ((!(text != "")) ? $"MapIcon_{entity.Index}_{entity.Version}" : ((text2 != null) ? $"MapIcon_{entity.Index}_{entity.Version}_({text} on Player {text2})" : ((text3 == null) ? $"MapIcon_{entity.Index}_{entity.Version}_({text})" : $"MapIcon_{entity.Index}_{entity.Version}_({text} on {text3})")));
			}
			else if (text.StartsWith("AB_"))
			{
				fileName = $"{text}_{entity.Index}_{entity.Version}";
				if (text2 != null)
				{
					fileName = $"{text}_{entity.Index}_{entity.Version}_(Attached to Player {text2})";
				}
				else if (text3 != null)
				{
					fileName = $"{text}_{entity.Index}_{entity.Version}_(Attached to {text3})";
				}
			}
			else if (entity.Has<CastleTerritory>())
			{
				CastleTerritory val6 = entity.Read<CastleTerritory>();
				fileName = $"CastleTerritory_{val6.CastleTerritoryIndex}_{entity.Index}_{entity.Version}";
			}
			else if (entity.Has<MapZoneData>())
			{
				MapZoneData val7 = entity.Read<MapZoneData>();
				fileName = $"MapZone_{val7.ZoneIndex}_{entity.Index}_{entity.Version}";
			}
			else if (entity.Has<WorldRegionPolygon>())
			{
				WorldRegionPolygon val8 = entity.Read<WorldRegionPolygon>();
				fileName = $"WorldRegionPolygon_{val8.WorldRegion}_{entity.Index}_{entity.Version}";
			}
		}
		goto IL_10d4;
		IL_10d4:
		if (!fileName.EndsWith(".txt"))
		{
			fileName += ".txt";
		}
		CopyStateFileToPrev(fileName);
		string text4;
		if (!useProjectMDump)
		{
			text4 = EntityDebug.RetrieveComponentData(entity);
		}
		else
		{
			StringBuilder val9 = new StringBuilder();
			EntityDebuggingUtility.DumpEntity(Core.TheWorld, entity, true, val9);
			text4 = ((Object)val9).ToString();
		}
		if (entity.Has<TeamAllies>())
		{
			EntityManager entityManager = Core.TheWorld.EntityManager;
			DynamicBuffer<TeamAllies> buffer = ((EntityManager)(ref entityManager)).GetBuffer<TeamAllies>(entity, false);
			for (int i = 0; i < buffer.Length; i++)
			{
				TeamAllies val10 = buffer[i];
				if (!((Entity)(ref val10.Value)).Equals(Entity.Null))
				{
					text4 = text4 + "\n\n#######################################\nTeam Ally - {i}\n#######################################\n" + EntityDebug.RetrieveComponentData(buffer[i].Value);
				}
			}
		}
		if (ctx != null && pasteBinKeys.TryGetValue(ctx.User.PlatformId, out var value))
		{
			ctx.Reply("For " + fileName + " Paste Bin Response: " + CreatePaste(value.ApiKey, value.UserKey, value.FolderKey, text4, fileName));
		}
		else
		{
			File.WriteAllText(Path.Combine(StateFolder, fileName), text4);
		}
		return fileName;
	}

	private static string CreatePaste(string apiKey, string userKey, string folderName, string pasteText, string pasteName)
	{
		HttpClient httpClient = new HttpClient();
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{ "api_dev_key", apiKey },
			{ "api_user_key", userKey },
			{ "api_option", "paste" },
			{ "api_paste_code", pasteText },
			{ "api_paste_name", pasteName },
			{ "api_folder_key", folderName },
			{ "api_paste_expire_date", "1H" }
		};
		if (string.IsNullOrEmpty(folderName))
		{
			dictionary.Remove("api_folder_key");
		}
		return httpClient.PostAsync("https://pastebin.com/api/api_post.php", new FormUrlEncodedContent(dictionary)).Result.Content.ReadAsStringAsync().Result;
	}

	[Command("SetPasteBinKeysNoLog", null, null, "Sets the Pastebin API, userKey, and optional folder keys", null, true)]
	public static void SetPasteBinKeys(ChatCommandContext ctx, string apiKey, string userKey, string folderKey = "")
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		pasteBinKeys[ctx.User.PlatformId] = new PasteBinKeys
		{
			ApiKey = apiKey,
			UserKey = userKey,
			FolderKey = folderKey
		};
		ctx.Reply("PasteBin keys set");
	}

	[Command("clan", "c", null, "Spits out clan info", null, true)]
	public static void ClanState(ChatCommandContext ctx, string clanName)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Directory.CreateDirectory(StateFolder);
		if (!Helper.FindClan(clanName, out var clanEntity))
		{
			ctx.Reply("No clan found matching name '" + clanName + "'");
			return;
		}
		string text = OutputEntityState(ctx, clanEntity);
		ctx.Reply("Clan '" + clanName + "' state written to " + text);
	}

	[Command("player", "p", null, "Removes a player from a clan", null, true)]
	public static void PlayerState(ChatCommandContext ctx, OnlinePlayer player = null)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		string value = player?.Value.CharacterName ?? ctx.Name;
		Entity entity = player?.Value.UserEntity ?? ctx.Event.SenderUserEntity;
		Entity entity2 = player?.Value.CharEntity ?? ctx.Event.SenderCharacterEntity;
		string value2 = OutputEntityState(ctx, entity);
		string value3 = OutputEntityState(ctx, entity2);
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(30, 3, stringBuilder2);
		handler.AppendLiteral("Player '");
		handler.AppendFormatted(value);
		handler.AppendLiteral("' state written to ");
		handler.AppendFormatted(value2);
		handler.AppendLiteral(", ");
		handler.AppendFormatted(value3);
		handler.AppendLiteral(",");
		stringBuilder3.Append(ref handler);
		if (entity2.Has<TeamReference>())
		{
			ModifiableEntity value4 = entity2.Read<TeamReference>().Value;
			string value5 = OutputEntityState(ctx, ModifiableEntity.op_Implicit(value4));
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
			handler.AppendLiteral(" ");
			handler.AppendFormatted(value5);
			handler.AppendLiteral(",");
			stringBuilder4.Append(ref handler);
		}
		NetworkedEntity progressionEntity = entity.Read<ProgressionMapper>().ProgressionEntity;
		string value6 = OutputEntityState(ctx, ((NetworkedEntity)(ref progressionEntity)).GetEntityOnServer());
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder5 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder2);
		handler.AppendLiteral(" and ");
		handler.AppendFormatted(value6);
		stringBuilder5.Append(ref handler);
		ctx.Reply(stringBuilder.ToString());
	}

	[Command("slots", "s", null, "Outputs all slots of the player", null, true)]
	public static void SlotsState(ChatCommandContext ctx, OnlinePlayer player = null)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		string text = player?.Value.CharacterName ?? ctx.Name;
		Entity val = player?.Value.CharEntity ?? ctx.Event.SenderCharacterEntity;
		EntityManager entityManager = Core.EntityManager;
		Enumerator<AttachedBuffer> enumerator = ((EntityManager)(ref entityManager)).GetBuffer<AttachedBuffer>(val, false).GetEnumerator();
		while (enumerator.MoveNext())
		{
			AttachedBuffer current = enumerator.Current;
			if (!(current.PrefabGuid != Prefabs.AbilityGroupSlot))
			{
				AbilityGroupSlot val2 = current.Entity.Read<AbilityGroupSlot>();
				OutputEntityState(ctx, current.Entity, $"{text}_Slot_{val2.SlotId}.txt");
			}
		}
		ctx.Reply("Player '" + text + "' slots saved");
	}

	[Command("inventory", "i", null, "Retrieves inventory state", null, true)]
	public unsafe static void InventoryState(ChatCommandContext ctx, OnlinePlayer player = null)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		string text = player?.Value.CharacterName ?? ctx.Name;
		Entity val = player?.Value.CharEntity ?? ctx.Event.SenderCharacterEntity;
		EntityManager entityManager = Core.EntityManager;
		Enumerator<InventoryInstanceElement> enumerator = ((EntityManager)(ref entityManager)).GetBuffer<InventoryInstanceElement>(val, false).GetEnumerator();
		while (enumerator.MoveNext())
		{
			InventoryInstanceElement current = enumerator.Current;
			NetworkedEntity externalInventoryEntity = current.ExternalInventoryEntity;
			object obj = Entity.Null;
			if (!((object)(*(NetworkedEntity*)(&externalInventoryEntity))/*cast due to .constrained prefix*/).Equals(obj))
			{
				externalInventoryEntity = current.ExternalInventoryEntity;
				OutputEntityState(ctx, ((NetworkedEntity)(ref externalInventoryEntity)).GetEntityOnServer());
			}
		}
		ctx.Reply("Player '" + text + "' inventory saved");
	}

	[Command("door", "d", null, "Outputs door states", null, true)]
	public static void DoorState(ChatCommandContext ctx, OnlinePlayer player = null)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		string value = player?.Value.CharacterName ?? ctx.Name;
		Entity val = player?.Value.UserEntity ?? ctx.Event.SenderUserEntity;
		int num = 0;
		Enumerator<Entity> enumerator = Helper.GetEntitiesByComponentType<Door>(includeAll: true).GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			if (current.Has<UserOwner>())
			{
				UserOwner val2 = current.Read<UserOwner>();
				Entity entityOnServer = ((NetworkedEntity)(ref val2.Owner)).GetEntityOnServer();
				if (((Entity)(ref entityOnServer)).Equals(val))
				{
					OutputEntityState(ctx, current);
					num++;
				}
			}
		}
		ctx.Reply($"Wrote out {num} door states for {value}");
	}

	[Command("ownedby", "o", null, "Outputs state of entities owned by the player", null, true)]
	public static void OwnedByState(ChatCommandContext ctx, OnlinePlayer player = null)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		string value = player?.Value.CharacterName ?? ctx.Name;
		Entity val = player?.Value.UserEntity ?? ctx.Event.SenderUserEntity;
		int num = 0;
		Enumerator<Entity> enumerator = Helper.GetEntitiesByComponentType<UserOwner>(includeAll: true).GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			UserOwner val2 = current.Read<UserOwner>();
			Entity entityOnServer = ((NetworkedEntity)(ref val2.Owner)).GetEntityOnServer();
			if (((Entity)(ref entityOnServer)).Equals(val))
			{
				OutputEntityState(ctx, current);
				num++;
			}
		}
		ctx.Reply($"Wrote out {num} states owned by {value}");
	}

	[Command("entity", "e", null, "Spits out entity info", null, true)]
	public static void EntityState(ChatCommandContext ctx, int id, int version = 1)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		Entity val = new Entity
		{
			Index = id,
			Version = version
		};
		EntityManager entityManager = Core.EntityManager;
		if (!((EntityManager)(ref entityManager)).Exists(val))
		{
			ctx.Reply($"No entity found with id {id}");
			return;
		}
		string value = OutputEntityState(ctx, val);
		ctx.Reply($"Entity {id} state written to {value}");
	}

	[Command("prefab", null, null, "Spits out entity info", null, true)]
	public static void PrefabState(ChatCommandContext ctx, int? id = null)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		PrefabCollectionSystem existingSystemManaged = Core.TheWorld.GetExistingSystemManaged<PrefabCollectionSystem>();
		if (!id.HasValue)
		{
			GameObject val = new GameObject("PrefabOutputter");
			((MonoBehaviour)val.AddComponent<IgnorePhysicsDebugSystem>()).StartCoroutine(CollectionExtensions.WrapToIl2Cpp(OutputtingAllPrefabs(ctx, existingSystemManaged, val)));
			return;
		}
		PrefabGUID val2 = new PrefabGUID
		{
			_Value = id.Value
		};
		PrefabLookupMap prefabLookupMap = existingSystemManaged._PrefabLookupMap;
		Entity entity = default(Entity);
		if (((PrefabLookupMap)(ref prefabLookupMap)).TryGetValue(val2, ref entity))
		{
			string value = OutputEntityState(ctx, entity, val2.LookupName() + ".txt");
			ctx.Reply($"Prefab {id} {val2.LookupName()} state written to {value}");
		}
		else
		{
			ctx.Reply($"Prefab doesn't exist for {id}");
		}
	}

	private static IEnumerator OutputtingAllPrefabs(ChatCommandContext ctx, PrefabCollectionSystem collectionSystem, GameObject gameObject)
	{
		Enumerator<PrefabGUID, Entity> enumerator = collectionSystem._PrefabGuidToEntityMap.GetEnumerator();
		Entity entity = default(Entity);
		bool flag = default(bool);
		while (enumerator.MoveNext())
		{
			PrefabGUID key = enumerator.Current.Key;
			NativeParallelHashMap<PrefabGUID, Entity> guidToEntityMap = collectionSystem._PrefabLookupMap.GuidToEntityMap;
			if (guidToEntityMap.TryGetValue(key, ref entity))
			{
				try
				{
					string text = OutputEntityState(ctx, entity, key.LookupName() + ".txt");
					ManualLogSource log = Core.Log;
					BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(26, 3, ref flag);
					if (flag)
					{
						((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Prefab ");
						((BepInExLogInterpolatedStringHandler)val).AppendFormatted<int>(((PrefabGUID)(ref key)).GuidHash);
						((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" ");
						((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
						((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" state written to ");
						((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(text);
					}
					log.LogInfo(val);
				}
				catch (Exception ex)
				{
					Core.LogException(ex, "Outputting Prefab " + key.LookupName());
					ManualLogSource log2 = Core.Log;
					BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(24, 3, ref flag);
					if (flag)
					{
						((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Error writing prefab ");
						((BepInExLogInterpolatedStringHandler)val).AppendFormatted<int>(((PrefabGUID)(ref key)).GuidHash);
						((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" ");
						((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
						((BepInExLogInterpolatedStringHandler)val).AppendLiteral(": ");
						((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(ex.Message);
					}
					log2.LogInfo(val);
				}
				yield return null;
			}
			else
			{
				ManualLogSource log3 = Core.Log;
				BepInExInfoLogInterpolatedStringHandler val = new BepInExInfoLogInterpolatedStringHandler(26, 2, ref flag);
				if (flag)
				{
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral("Prefab doesn't exist for ");
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<int>(((PrefabGUID)(ref key)).GuidHash);
					((BepInExLogInterpolatedStringHandler)val).AppendLiteral(" ");
					((BepInExLogInterpolatedStringHandler)val).AppendFormatted<string>(key.LookupName());
				}
				log3.LogInfo(val);
			}
		}
		Object.Destroy((Object)(object)gameObject);
	}

	[Command("teams", "t", null, "Checks all the team data", null, true)]
	public static void TeamState(ChatCommandContext ctx)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> entitiesByComponentType = Helper.GetEntitiesByComponentType<TeamData>(includeAll: true);
		Enumerator<Entity> enumerator = entitiesByComponentType.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			OutputEntityState(ctx, current);
		}
		ctx.Reply($"{entitiesByComponentType.Length} teams written to files");
	}

	[Command("nearby", "n", null, "Gets nearby entities", null, true)]
	public static void NearbyEntityStates(ChatCommandContext ctx, int radius = 10)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Entity senderUserEntity = ctx.Event.SenderUserEntity;
		float3 value = senderUserEntity.Read<Translation>().Value;
		NativeArray<Entity> entitiesByComponentType = Helper.GetEntitiesByComponentType<Translation>(includeAll: true);
		int num = 0;
		Enumerator<Entity> enumerator = entitiesByComponentType.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			float3 value2 = current.Read<Translation>().Value;
			if (!(Vector3.Distance(float3.op_Implicit(value), float3.op_Implicit(value2)) > (float)radius))
			{
				OutputEntityState(ctx, current);
				num++;
			}
		}
		ctx.Reply($"Wrote out {num} states within {radius} units of {senderUserEntity.Index}");
	}

	[Command("tilemodels", "tm", null, "Gets nearby tile model entities", null, true)]
	public static void NearbyTileModelStates(ChatCommandContext ctx, int radius = 10)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Entity senderUserEntity = ctx.Event.SenderUserEntity;
		float3 value = senderUserEntity.Read<Translation>().Value;
		NativeArray<Entity> entitiesByComponentType = Helper.GetEntitiesByComponentType<TileModel>(includeAll: false, includeDisabled: true, includeSpawn: true);
		int num = 0;
		Enumerator<Entity> enumerator = entitiesByComponentType.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			if (current.Has<Translation>())
			{
				float3 value2 = current.Read<Translation>().Value;
				if (!(Vector3.Distance(float3.op_Implicit(value), float3.op_Implicit(value2)) > (float)radius))
				{
					OutputEntityState(ctx, current);
					num++;
				}
			}
		}
		ctx.Reply($"Wrote out {num} states for Tile Models within {radius} units of {senderUserEntity.Index}");
	}

	[Command("rooms", "r", null, "Gets nearby rooms", null, true)]
	public static void NearbyRoomStates(ChatCommandContext ctx, int radius = 10)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Entity senderUserEntity = ctx.Event.SenderUserEntity;
		float3 value = senderUserEntity.Read<Translation>().Value;
		NativeArray<Entity> entitiesByComponentTypes = Helper.GetEntitiesByComponentTypes<Translation, CastleRoom>(includeAll: true);
		int num = 0;
		Enumerator<Entity> enumerator = entitiesByComponentTypes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			float3 value2 = current.Read<Translation>().Value;
			if (!(Vector3.Distance(float3.op_Implicit(value), float3.op_Implicit(value2)) > (float)radius))
			{
				OutputEntityState(ctx, current);
				num++;
			}
		}
		ctx.Reply($"Wrote out {num} states within {radius} units of {senderUserEntity.Index}");
	}

	[Command("castleterritory", "ct", null, "Outputs a particular or all castle territory", null, true)]
	public static void CastleTerritoryState(ChatCommandContext ctx, int? id = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> entitiesByComponentType = Helper.GetEntitiesByComponentType<CastleTerritory>(includeAll: true);
		if (!id.HasValue)
		{
			Enumerator<Entity> enumerator = entitiesByComponentType.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				OutputEntityState(ctx, current);
			}
			ctx.Reply($"{entitiesByComponentType.Length} castle territories written to files");
			return;
		}
		Entity entity = ((IEnumerable<Entity>)entitiesByComponentType.ToArray()).FirstOrDefault((Func<Entity, bool>)((Entity e) => e.Read<CastleTerritory>().CastleTerritoryIndex == id));
		if (((Entity)(ref entity)).Equals(Entity.Null))
		{
			ctx.Reply($"No castle territory found with id {id}");
		}
		else
		{
			OutputEntityState(ctx, entity);
			ctx.Reply($"Castle territory {id} state written to file");
		}
	}

	[Command("mapzones", "mz", null, "Outputs map zones out", null, true)]
	public static void MapZoneState(ChatCommandContext ctx)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> entitiesByComponentType = Helper.GetEntitiesByComponentType<MapZoneData>(includeAll: true);
		Enumerator<Entity> enumerator = entitiesByComponentType.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			OutputEntityState(ctx, current);
		}
		ctx.Reply($"{entitiesByComponentType.Length} map zones written to files");
	}

	[Command("worldregionpolygon", "wrp", null, "Outputs all world region polygons", null, true)]
	public static void WorldRegionPolygonState(ChatCommandContext ctx)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> entitiesByComponentType = Helper.GetEntitiesByComponentType<WorldRegionPolygon>(includeAll: true);
		Enumerator<Entity> enumerator = entitiesByComponentType.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			OutputEntityState(ctx, current);
		}
		ctx.Reply($"{entitiesByComponentType.Length} world region polygons written to files");
	}

	[Command("chunkportals", "cp", null, "Outputs all chunk portals", null, true)]
	public static void ChunkPortalState(ChatCommandContext ctx)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> entitiesByComponentType = Helper.GetEntitiesByComponentType<ChunkPortal>(includeAll: true);
		Enumerator<Entity> enumerator = entitiesByComponentType.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			OutputEntityState(ctx, current);
		}
		ctx.Reply($"{entitiesByComponentType.Length} chunk portals written to files");
	}

	[Command("buffs", "b", null, "Outputs all buffs of nearby entities", null, true)]
	public static void BuffStates(ChatCommandContext ctx, int radius = 5)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Entity senderUserEntity = ctx.Event.SenderUserEntity;
		float3 value = senderUserEntity.Read<Translation>().Value;
		NativeArray<Entity> entitiesByComponentType = Helper.GetEntitiesByComponentType<Buff>(includeAll: true);
		int num = 0;
		Enumerator<Entity> enumerator = entitiesByComponentType.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			if (!current.Has<Attach>())
			{
				continue;
			}
			Attach val = current.Read<Attach>();
			Entity parent = val.Parent;
			if (!((Entity)(ref parent)).Equals(Entity.Null))
			{
				float3 value2 = val.Parent.Read<Translation>().Value;
				if (!(Vector3.Distance(float3.op_Implicit(value), float3.op_Implicit(value2)) > (float)radius))
				{
					OutputEntityState(ctx, current);
					num++;
				}
			}
		}
		ctx.Reply($"Wrote out {num} buff states within {radius} units of {senderUserEntity.Index}");
	}

	[Command("spawnregions", "sr", null, "Outputs all spawn regions", null, true)]
	public static void SpawnRegionState(ChatCommandContext ctx)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> entitiesByComponentType = Helper.GetEntitiesByComponentType<SpawnRegion>(includeAll: true);
		Enumerator<Entity> enumerator = entitiesByComponentType.GetEnumerator();
		bool flag = default(bool);
		while (enumerator.MoveNext())
		{
			Entity current = enumerator.Current;
			LocalToWorld val = current.Read<LocalToWorld>();
			SpawnRegion val2 = current.Read<SpawnRegion>();
			ManualLogSource log = Core.Log;
			BepInExInfoLogInterpolatedStringHandler val3 = new BepInExInfoLogInterpolatedStringHandler(4, 3, ref flag);
			if (flag)
			{
				((BepInExLogInterpolatedStringHandler)val3).AppendFormatted<float3>(((LocalToWorld)(ref val)).Position);
				((BepInExLogInterpolatedStringHandler)val3).AppendLiteral(", ");
				((BepInExLogInterpolatedStringHandler)val3).AppendFormatted<float>(val2.RespawnDurationMin);
				((BepInExLogInterpolatedStringHandler)val3).AppendLiteral(", ");
				((BepInExLogInterpolatedStringHandler)val3).AppendFormatted<float>(val2.RespawnDurationMax);
			}
			log.LogInfo(val3);
			OutputEntityState(ctx, current);
		}
		ctx.Reply($"{entitiesByComponentType.Length} spawn regions written to files");
	}

	[Command("time", null, null, "Outputs the current time", null, true)]
	public static void TimeState(ChatCommandContext ctx)
	{
		double serverTime = Core.ServerTime;
		ctx.Reply($"Current time: {serverTime}");
	}
}
