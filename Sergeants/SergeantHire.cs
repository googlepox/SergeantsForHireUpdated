using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Sergeants
{
	internal class SergeantHire : CampaignBehaviorBase
	{
		private void OnSessionLaunched(CampaignGameStarter obj)
		{
			try
			{
				this.add_recruit_menu(obj);
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("Sergeants available for hire in cities!", null).ToString(), Colors.Red));
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject(ex.ToString(), null).ToString(), Colors.Red));
			}
		}
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}
		public override void SyncData(IDataStore dataStore)
		{
		}
		public void giveTroop(SergeantHire.sergeant_types st)
		{
			CharacterObject unit;
			if (TaleWorlds.Core.Extensions.NextFloat(new Random()) < ConfigLoader.Instance.Config.male_fraction)
			{
				switch (st)
				{
				case SergeantHire.sergeant_types.infantry:
					unit = CharacterObject.Find(SergeantHire.sergeant_ids[0]);
					break;
				case SergeantHire.sergeant_types.mounted:
					unit = CharacterObject.Find(SergeantHire.sergeant_ids[1]);
					break;
				case SergeantHire.sergeant_types.ranged:
					unit = CharacterObject.Find(SergeantHire.sergeant_ids[2]);
					break;
				case SergeantHire.sergeant_types.warden:
					unit = CharacterObject.Find(SergeantHire.sergeant_ids[3]);
					break;
				default:
					unit = CharacterObject.Find(SergeantHire.sergeant_ids[0]);
					break;
				}
			}
			else
			{
				switch (st)
				{
				case SergeantHire.sergeant_types.infantry:
					unit = CharacterObject.Find(SergeantHire.f_sergeant_ids[0]);
					break;
				case SergeantHire.sergeant_types.mounted:
					unit = CharacterObject.Find(SergeantHire.f_sergeant_ids[1]);
					break;
				case SergeantHire.sergeant_types.ranged:
					unit = CharacterObject.Find(SergeantHire.f_sergeant_ids[2]);
					break;
				case SergeantHire.sergeant_types.warden:
					unit = CharacterObject.Find(SergeantHire.f_sergeant_ids[3]);
					break;
				default:
					unit = CharacterObject.Find(SergeantHire.f_sergeant_ids[0]);
					break;
				}
			}
			MobileParty.MainParty.AddElementToMemberRoster(unit, 1, false);
		}
		private static bool inf_cond(MenuCallbackArgs args)
		{
			return SergeantHire.on_conditions(0, args);
		}
		private static bool mnt_cond(MenuCallbackArgs args)
		{
			return SergeantHire.on_conditions(1, args);
		}
		private static bool ran_cond(MenuCallbackArgs args)
		{
			return SergeantHire.on_conditions(2, args);
		}
		private static bool war_cond(MenuCallbackArgs args)
		{
			return SergeantHire.on_conditions(3, args);
		}
		private static bool on_conditions(int mode, MenuCallbackArgs args)
		{
			int content = 0;
			if (mode == 0)
			{
				content = (int)Math.Ceiling((double)ConfigLoader.Instance.Config.infantry_sergeant_cost);
				int extra = (int)(ConfigLoader.Instance.Config.infantry_sergeant_hire_scale * Clan.PlayerClan.Renown);
				content += extra;
				args.Tooltip = new TextObject("Infantry sergeants are skilled with melee weapons and are ideally suited to being stuck in with infantry units. They maintain morale of units in their formations during combat.", null);
				MBTextManager.SetTextVariable("COST_INF", content);
			}
			else if (mode == 1)
			{
				content = (int)Math.Ceiling((double)ConfigLoader.Instance.Config.mounted_sergeant_cost);
				int extra = (int)(ConfigLoader.Instance.Config.mounted_sergeant_hire_scale * Clan.PlayerClan.Renown);
				content += extra;
				args.Tooltip = new TextObject("Mounted sergeants are skilled with horses and are ideally suited to being stuck in with cavalry units. They maintain morale of units in their formations during combat.", null);
				MBTextManager.SetTextVariable("COST_CAV", content);
			}
			else if (mode == 2)
			{
				content = (int)Math.Ceiling((double)ConfigLoader.Instance.Config.ranged_sergeant_cost);
				int extra = (int)(ConfigLoader.Instance.Config.ranged_sergeant_hire_scale * Clan.PlayerClan.Renown);
				content += extra;
				args.Tooltip = new TextObject("Ranged sergeants are skilled with ranged weapons and are ideally suited to being stuck in with archer units. They maintain morale of units in their formations during combat.", null);
				MBTextManager.SetTextVariable("COST_RAN", content);
			}
			else if (mode == 3)
			{
				content = (int)Math.Ceiling((double)ConfigLoader.Instance.Config.warden_cost);
				int extra = (int)(ConfigLoader.Instance.Config.warden_hire_scale * Clan.PlayerClan.Renown);
				content += extra;
				args.Tooltip = new TextObject("Wardens use blunt weapons and are skilled melee combatants. They allow you to handle more prisoners.", null);
				MBTextManager.SetTextVariable("COST_WAR", content);
			}
			return content < Hero.MainHero.Gold;
		}
		public void add_recruit_menu(CampaignGameStarter obj)
		{
			obj.AddGameMenuOption("town", "info_troop_type", "Hire Sergeants", new GameMenuOption.OnConditionDelegate(this.game_menu_just_add_recruit_conditional), delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("town_mod_pay");
			}, false, 5, false);
			obj.AddGameMenu("town_mod_pay", "The Master Sergeant shows you the soldiers they have available to hire. They specialize in organization and command, lightening the load you need to shoulder to lead troops effectively. They will increase the number of soldiers you can command.\n\n Wardens are skilled at intimidating and managing prisoners, and will keep them in line while you focus on more important things.", delegate(MenuCallbackArgs args)
			{
				if (Clan.PlayerClan.Tier != 0)
				{
					return;
				}
				MBTextManager.SetTextVariable("RENOWN_STATUS", "No one here has heard of you before", false);
			}, 0, 0, null);
			obj.AddGameMenuOption("town_mod_pay", "notpaying", "Leave", new GameMenuOption.OnConditionDelegate(this.game_menu_just_add_leave_conditional), new GameMenuOption.OnConsequenceDelegate(this.game_menu_switch_to_town_menu), false, -1, false);
			obj.AddGameMenuOption("town_mod_pay", "pay_fee_inf", "Pay {COST_INF} {GOLD_ICON} for an infantry sergeant", new GameMenuOption.OnConditionDelegate(SergeantHire.inf_cond), delegate(MenuCallbackArgs args)
			{
				int amount = (int)Math.Ceiling((double)ConfigLoader.Instance.Config.infantry_sergeant_cost);
				int extra = (int)(ConfigLoader.Instance.Config.infantry_sergeant_hire_scale * Clan.PlayerClan.Renown);
				amount += extra;
				if (amount > Hero.MainHero.Gold)
				{
					return;
				}
				GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, amount, false);
				this.giveTroop(SergeantHire.sergeant_types.infantry);
			}, false, -1, false);
			obj.AddGameMenuOption("town_mod_pay", "pay_fee_cav", "Pay {COST_CAV} {GOLD_ICON} for a cavalry sergeant", new GameMenuOption.OnConditionDelegate(SergeantHire.mnt_cond), delegate(MenuCallbackArgs args)
			{
				int amount = (int)Math.Ceiling((double)ConfigLoader.Instance.Config.mounted_sergeant_cost);
				int extra = (int)(ConfigLoader.Instance.Config.mounted_sergeant_hire_scale * Clan.PlayerClan.Renown);
				amount += extra;
				if (amount > Hero.MainHero.Gold)
				{
					return;
				}
				GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, amount, false);
				this.giveTroop(SergeantHire.sergeant_types.mounted);
			}, false, -1, false);
			obj.AddGameMenuOption("town_mod_pay", "pay_fee_ran", "Pay {COST_RAN} {GOLD_ICON} for a ranged sergeant", new GameMenuOption.OnConditionDelegate(SergeantHire.ran_cond), delegate(MenuCallbackArgs args)
			{
				int amount = (int)Math.Ceiling((double)ConfigLoader.Instance.Config.ranged_sergeant_cost);
				int extra = (int)(ConfigLoader.Instance.Config.ranged_sergeant_hire_scale * Clan.PlayerClan.Renown);
				amount += extra;
				if (amount > Hero.MainHero.Gold)
				{
					return;
				}
				GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, amount, false);
				this.giveTroop(SergeantHire.sergeant_types.ranged);
			}, false, -1, false);
			obj.AddGameMenuOption("town_mod_pay", "pay_fee_war", "Pay {COST_WAR} {GOLD_ICON} for a warden", new GameMenuOption.OnConditionDelegate(SergeantHire.war_cond), delegate(MenuCallbackArgs args)
			{
				int amount = (int)Math.Ceiling((double)ConfigLoader.Instance.Config.warden_cost);
				int extra = (int)(ConfigLoader.Instance.Config.warden_hire_scale * Clan.PlayerClan.Renown);
				amount += extra;
				if (amount > Hero.MainHero.Gold)
				{
					return;
				}
				GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, amount, false);
				this.giveTroop(SergeantHire.sergeant_types.warden);
			}, false, -1, false);
		}
		private void game_menu_switch_to_town_menu(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town");
		}
		private bool game_menu_just_add_recruit_conditional(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
			return true;
		}
		private bool game_menu_just_add_leave_conditional(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}
		public static string[] sergeant_ids = new string[]
		{
			"sergeant_inf",
			"sergeant_cav",
			"sergeant_ran",
			"warden_inf"
		};
		public static string[] f_sergeant_ids = new string[]
		{
			"f_sergeant_inf",
			"f_sergeant_cav",
			"f_sergeant_ran",
			"f_warden_inf"
		};
		public enum sergeant_types
		{
			infantry,
			mounted,
			ranged,
			warden
		}
	}
}
