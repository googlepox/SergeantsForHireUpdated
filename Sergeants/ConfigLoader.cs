using System;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Sergeants
{
	public class ConfigLoader
	{
		public Config Config { get; private set; }
		public static ConfigLoader Instance
		{
			get
			{
				if (ConfigLoader.instance == null)
				{
					ConfigLoader.instance = new ConfigLoader();
				}
				return ConfigLoader.instance;
			}
		}
		private ConfigLoader()
		{
			this.Config = this.getConfig(Path.Combine(BasePath.Name, "Modules", "Sergeants", "Settings.xml"));
		}
		private Config getConfig(string filePath)
		{
			Config result;
			try
			{
				using (StreamReader streamReader = new StreamReader(filePath))
				{
					result = (Config)new XmlSerializer(typeof(Config)).Deserialize(streamReader);
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage("Failed to load config, using default native values due to: " + ex.Message, Colors.Red));
				result = new Config
				{
					infantry_sergeant_cost = 7500,
					mounted_sergeant_cost = 10000,
					ranged_sergeant_cost = 7500,
					warden_cost = 5000,
					infantry_sergeant_hire_scale = 1f,
					mounted_sergeant_hire_scale = 1f,
					ranged_sergeant_hire_scale = 1f,
					warden_hire_scale = 0.25f,
					sergeant_party_size_bonus = 25,
					lieutenant_bonus_size = 10,
					warden_prisoner_size_bonus = 25,
					male_fraction = 1f,
					sergeant_morale_boost_delay = 10f,
					sergeant_morale_boost_amount = 10f,
					lieutenant_morale_boost_factor = 1.5f
				};
			}
			return result;
		}
		public static ConfigLoader instance;
	}
}
