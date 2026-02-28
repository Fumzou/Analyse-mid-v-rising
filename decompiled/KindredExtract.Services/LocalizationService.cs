using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Il2CppSystem.Collections.Generic;
using Stunlock.Core;
using Stunlock.Localization;

namespace KindredExtract.Services;

internal class LocalizationService
{
	private Dictionary<string, string> localization = new Dictionary<string, string>();

	public LocalizationService()
	{
		LoadLocalization();
	}

	private void LoadLocalization()
	{
		string name = "KindredExtract.Localization.English.json";
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
		if (manifestResourceStream != null)
		{
			using (StreamReader streamReader = new StreamReader(manifestResourceStream))
			{
				string json = streamReader.ReadToEnd();
				localization = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
				return;
			}
		}
		Console.WriteLine("Resource not found!");
	}

	public void SaveLocalization()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		localization.Clear();
		Enumerator<AssetGuid, string> enumerator = Localization._LocalizedStrings.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<AssetGuid, string> current = enumerator.Current;
			Dictionary<string, string> dictionary = localization;
			AssetGuid key = current.Key;
			dictionary[((object)((AssetGuid)(ref key)).ToGuid()/*cast due to .constrained prefix*/).ToString()] = current.Value;
		}
		string contents = JsonSerializer.Serialize(localization);
		File.WriteAllText(Localization.CurrentLanguage + ".json", contents);
	}

	public string GetLocalization(string guid)
	{
		if (localization.TryGetValue(guid, out var value))
		{
			return value;
		}
		return "<Localization not found!>";
	}
}
