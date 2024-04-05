using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Localization;

namespace Sergeants
{
    [HarmonyPatch]
    internal class SergeantSizeLimitPatch
    {
        private static IEnumerable<MethodBase> TargetMethods(Harmony h)
        {
            new List<MethodInfo>();
            foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assem.GetTypes())
                {
                    MethodInfo mi = t.GetMethod("GetPartyMemberSizeLimit");
                    if (mi != null && !mi.IsAbstract && !(mi.ReturnType == typeof(void)))
                    {
                        yield return mi;
                        FileLog.Log("Found " + mi.Name + " in " + t.Name);
                    }
                    mi = null;
                }
            }
            yield break;
        }
        [HarmonyPostfix]
        public static void Postfix(ref ExplainedNumber __result, PartyBase party)
        {
            if (!party.IsMobile)
            {
                return;
            }
            int n_sergeants = 0;
            foreach (TroopRosterElement troopRosterElement in party.MemberRoster.GetTroopRoster())
            {
                CharacterObject co = troopRosterElement.Character;
                if (co.Occupation == Occupation.Special)
                {
                    int n_to_add = party.MemberRoster.GetTroopCount(co);
                    n_sergeants += n_to_add;
                    int n_extra_from_tier = ConfigLoader.Instance.Config.lieutenant_bonus_size * (co.Tier - 5);
                    float num = (float)(n_to_add * (ConfigLoader.Instance.Config.sergeant_party_size_bonus + n_extra_from_tier));
                    string str = n_to_add.ToString();
                    string str2 = " x ";
                    TextObject name = co.Name;
                    __result.Add(num, new TextObject(str + str2 + ((name != null) ? name.ToString() : null), null), null);
                }
            }
        }
    }
}
