using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Sergeants
{
	public class SubModule : MBSubModuleBase
	{
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			Harmony.DEBUG = false;
			FileLog.Reset();
			Harmony harmony = new Harmony("mod.bannerlord.sergeants");
			try
			{
				harmony.PatchAll();
			}
			catch (Exception ex)
			{
				string str = "Error patching:\n";
				string message = ex.Message;
				string str2 = " \n\n";
				Exception innerException = ex.InnerException;
				FileLog.Log(str + message + str2 + ((innerException != null) ? innerException.Message : null));
			}
		}
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
		}
		public override void OnGameLoaded(Game game, object initializerObject)
		{
			if (!(game.GameType is Campaign))
			{
				return;
			}
			((CampaignGameStarter)initializerObject).AddBehavior(new SergeantHire());
			((CampaignGameStarter)initializerObject).AddBehavior(new SergeantBehaviour());
		}
		public override void OnCampaignStart(Game game, object starterObject)
		{
			if (!(game.GameType is Campaign))
			{
				return;
			}
			((CampaignGameStarter)starterObject).AddBehavior(new SergeantHire());
			((CampaignGameStarter)starterObject).AddBehavior(new SergeantBehaviour());
		}
	}
}
