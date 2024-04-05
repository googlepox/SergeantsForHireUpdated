using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Sergeants
{
	public class SergeantBehaviour : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.FindBattle));
		}
		public override void SyncData(IDataStore dataStore)
		{
		}
		public void FindBattle(IMission misson)
		{
			if (((Mission)misson).CombatType > 0 || !(Mission.Current.Scene != null))
			{
				return;
			}
			Mission.Current.AddMissionBehavior(new SergeantBattleEffects());
		}
	}
}
