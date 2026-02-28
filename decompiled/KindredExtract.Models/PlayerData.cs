using Unity.Entities;

namespace KindredExtract.Models;

public struct PlayerData
{
	public string CharacterName { get; set; }

	public ulong SteamID { get; set; }

	public bool IsOnline { get; set; }

	public Entity UserEntity { get; set; }

	public Entity CharEntity { get; set; }

	public PlayerData(string characterName = null, ulong steamID = 0uL, bool isOnline = false, Entity userEntity = default(Entity), Entity charEntity = default(Entity))
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		CharacterName = characterName;
		SteamID = steamID;
		IsOnline = isOnline;
		UserEntity = userEntity;
		CharEntity = charEntity;
	}
}
