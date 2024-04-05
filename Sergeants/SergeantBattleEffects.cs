using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Sergeants
{
    internal class SergeantBattleEffects : MissionLogic
    {
        public override void AfterStart()
        {
            this._nextMoraleBoostTime = MissionTime.SecondsFromNow(ConfigLoader.Instance.Config.sergeant_morale_boost_delay);
        }
        public override void OnMissionTick(float dt)
        {
            if (!this._nextMoraleBoostTime.IsPast)
            {
                return;
            }
            this._nextMoraleBoostTime = MissionTime.SecondsFromNow(ConfigLoader.Instance.Config.sergeant_morale_boost_delay);
            foreach (Team team in base.Mission.Teams)
            {
                foreach (Formation f in team.FormationsIncludingSpecialAndEmpty)
                {
                    if (f.HasUnitsWithCondition(new Func<Agent, bool>(this.agent_is_sergeant)))
                    {
                        f.ApplyActionOnEachUnit(new Action<Agent>(this.give_agent_morale));
                    }
                    if (f.HasUnitsWithCondition(new Func<Agent, bool>(this.agent_is_lieutenant)))
                    {
                        f.ApplyActionOnEachUnit(new Action<Agent>(this.give_agent_double_morale));
                    }
                }
            }
        }
        private bool agent_is_sergeant(Agent a)
        {
            BasicCharacterObject bco = a.Character;
            CharacterObject characterObject = CharacterObject.Find(bco.StringId);
            if (characterObject != null && characterObject.Occupation == Occupation.Special)
            {
                CharacterObject characterObject2 = CharacterObject.Find(bco.StringId);
                return characterObject2 != null && characterObject2.Tier <= 5;
            }
            return false;
        }
        private bool agent_is_lieutenant(Agent a)
        {
            BasicCharacterObject bco = a.Character;
            CharacterObject characterObject = CharacterObject.Find(bco.StringId);
            if (characterObject != null && characterObject.Occupation == Occupation.Special)
            {
                CharacterObject characterObject2 = CharacterObject.Find(bco.StringId);
                return characterObject2 != null && characterObject2.Tier > 5;
            }
            return false;
        }
        private void give_agent_morale(Agent a)
        {
            AgentComponentExtensions.ChangeMorale(a, ConfigLoader.Instance.Config.sergeant_morale_boost_amount);
        }
        private void give_agent_double_morale(Agent a)
        {
            AgentComponentExtensions.ChangeMorale(a, ConfigLoader.Instance.Config.lieutenant_morale_boost_factor * ConfigLoader.Instance.Config.sergeant_morale_boost_amount);
        }
        private MissionTime _nextMoraleBoostTime;
    }
}
