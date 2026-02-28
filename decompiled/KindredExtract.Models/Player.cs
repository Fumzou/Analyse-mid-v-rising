using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace KindredExtract.Models;

public class Player
{
	public string Name { get; set; }

	public ulong SteamID { get; set; }

	public bool IsOnline { get; set; }

	public bool IsAdmin { get; set; }

	public Entity User { get; set; }

	public Entity Character { get; set; }

	public unsafe Player(Entity userEntity = default(Entity), Entity charEntity = default(Entity))
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		User = userEntity;
		User val = User.Read<User>();
		Character = val.LocalCharacter._Entity;
		Name = ((object)(*(FixedString64Bytes*)(&val.CharacterName))/*cast due to .constrained prefix*/).ToString();
		IsOnline = val.IsConnected;
		IsAdmin = val.IsAdmin;
		SteamID = val.PlatformId;
	}
}
