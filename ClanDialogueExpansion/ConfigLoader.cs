using System;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.Library;

namespace ClanDialogueExpansion;

internal sealed class ConfigLoader
{
	private static ConfigLoader _instance;

	public static ConfigLoader Instance => _instance ?? (_instance = new ConfigLoader());

	public Config Config { get; }

	private ConfigLoader()
	{
		Config = Load();
	}

	private static Config Load()
	{
		try
		{
			string path = Path.Combine(BasePath.Name, "Modules", "ClanDialogueExpansion", "ModuleData", "config.xml");
			if (File.Exists(path))
			{
				using StreamReader reader = new StreamReader(path);
				return (Config)new XmlSerializer(typeof(Config)).Deserialize(reader);
			}
		}
		catch (Exception)
		{
			// Defaults keep the mod loadable when an older installation has no config.
		}
		return new Config();
	}
}
