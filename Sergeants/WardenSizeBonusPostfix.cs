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
    internal class WardenSizeBonusPostfix
    {
        private static IEnumerable<MethodBase> TargetMethods(Harmony h)
        {
            new List<MethodInfo>();
            foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] array2 = assem.GetTypes();
                for (int j = 0; j < array2.Length; j++)
                {
                    MethodInfo mi = array2[j].GetMethod("GetPartyPrisonerSizeLimit");
                    if (mi != null && !mi.IsAbstract)
                    {
                        yield return mi;
                    }
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
                if (co.Occupation == Occupation.PrisonGuard)
                {
                    int n_to_add = party.MemberRoster.GetTroopCount(co);
                    n_sergeants += n_to_add;
                    float num = (float)(n_to_add * ConfigLoader.Instance.Config.sergeant_party_size_bonus);
                    string str = n_to_add.ToString();
                    string str2 = " x ";
                    TextObject name = co.Name;
                    __result.Add(num, new TextObject(str + str2 + ((name != null) ? name.ToString() : null), null), null);
                }
            }
        }
    }
}
