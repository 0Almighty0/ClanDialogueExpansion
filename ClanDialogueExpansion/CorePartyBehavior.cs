using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;
using Helpers;
using HarmonyLib;

namespace ClanDialogueExpansion;

public sealed class CorePartyBehavior : CampaignBehaviorBase
{
	public sealed class SaveDefiner : SaveableTypeDefiner
	{
		public SaveDefiner()
			: base(56335810)
		{
		}

		protected override void DefineClassTypes()
		{
			AddClassDefinition(typeof(PartyOrder), 56335811);
		}

		protected override void DefineEnumTypes()
		{
			AddEnumDefinition(typeof(CoreOrderType), 56335812);
		}

		protected override void DefineContainerDefinitions()
		{
			ConstructContainerDefinition(typeof(Dictionary<Hero, PartyOrder>));
			ConstructContainerDefinition(typeof(Dictionary<Hero, TroopRoster>));
		}
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static ConversationSentence.OnConditionDelegate _003C0_003E__HasEligibleSettlement;
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class LegacyC
	{
		public static readonly LegacyC _003C_003E9 = new LegacyC();

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__20_5;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__20_7;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__20_8;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__20_9;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__20_10;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__20_12;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__20_14;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__20_15;

		public static Func<PartyOrder, bool> _003C_003E9__21_0;

		public static Action<PartyOrder> _003C_003E9__21_1;

		public static Func<PartyOrder, bool> _003C_003E9__21_2;

		public static Action<PartyOrder> _003C_003E9__21_3;

		public static Func<PartyOrder, bool> _003C_003E9__21_4;

		public static Action<PartyOrder> _003C_003E9__21_5;

		public static Func<PartyOrder, bool> _003C_003E9__21_6;

		public static Action<PartyOrder> _003C_003E9__21_7;

		public static Func<PartyOrder, bool> _003C_003E9__21_8;

		public static Action<PartyOrder> _003C_003E9__21_9;

		public static Func<Settlement, bool> _003C_003E9__24_0;

		public static Func<Settlement, string> _003C_003E9__24_1;

		internal void _003CRegisterDialogue_003Eb__20_5()
		{
			SetOrderResponse(CdeText.Get("{=cde.core.response.nevermind}As you wish."));
		}

		internal void _003CRegisterDialogue_003Eb__20_7()
		{
			PartyTransferScreen.Open(Hero.OneToOneConversationHero.PartyBelongedTo);
		}

		internal bool _003CRegisterDialogue_003Eb__20_8()
		{
			return !HasEligibleSettlement();
		}

		internal bool _003CRegisterDialogue_003Eb__20_9()
		{
			return !HasEligibleSettlement();
		}

		internal bool _003CRegisterDialogue_003Eb__20_10()
		{
			return HasMoreThanOnePage();
		}

		internal bool _003CRegisterDialogue_003Eb__20_12()
		{
			return HasMoreThanOnePage();
		}

		internal void _003CRegisterDialogue_003Eb__20_14()
		{
			SetOrderResponse(CdeText.Get("{=cde.core.response.wait}Of course. I will await your next orders."));
		}

		internal void _003CRegisterDialogue_003Eb__20_15()
		{
			SetOrderResponse(CdeText.Get("{=cde.core.response.wait}Of course. I will await your next orders."));
		}

		internal bool _003CRegisterRuleLines_003Eb__21_0(PartyOrder x)
		{
			return x.StopRecruitingTroops;
		}

		internal void _003CRegisterRuleLines_003Eb__21_1(PartyOrder x)
		{
			x.StopRecruitingTroops = !x.StopRecruitingTroops;
		}

		internal bool _003CRegisterRuleLines_003Eb__21_2(PartyOrder x)
		{
			return x.StopTakingPrisoners;
		}

		internal void _003CRegisterRuleLines_003Eb__21_3(PartyOrder x)
		{
			x.StopTakingPrisoners = !x.StopTakingPrisoners;
		}

		internal bool _003CRegisterRuleLines_003Eb__21_4(PartyOrder x)
		{
			return !x.AllowRaidingVillages;
		}

		internal void _003CRegisterRuleLines_003Eb__21_5(PartyOrder x)
		{
			x.AllowRaidingVillages = !x.AllowRaidingVillages;
		}

		internal bool _003CRegisterRuleLines_003Eb__21_6(PartyOrder x)
		{
			return !x.AllowSieges;
		}

		internal void _003CRegisterRuleLines_003Eb__21_7(PartyOrder x)
		{
			x.AllowSieges = !x.AllowSieges;
		}

		internal bool _003CRegisterRuleLines_003Eb__21_8(PartyOrder x)
		{
			return !x.AllowJoiningArmies;
		}

		internal void _003CRegisterRuleLines_003Eb__21_9(PartyOrder x)
		{
			x.AllowJoiningArmies = !x.AllowJoiningArmies;
		}

		internal bool _003CGetPlayerOwnedSettlements_003Eb__24_0(Settlement x)
		{
			if (x != null && x.IsActive && (x.IsTown || x.IsCastle))
			{
				return x.MapFaction == Hero.MainHero.MapFaction;
			}
			return false;
		}

		internal string _003CGetPlayerOwnedSettlements_003Eb__24_1(Settlement x)
		{
			return x.Name.ToString();
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class ModernC
	{
		public static readonly ModernC _003C_003E9 = new ModernC();

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__23_8;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__23_9;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__23_10;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__23_11;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__23_16;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__23_17;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__23_18;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__23_19;

		public static Func<PartyOrder, bool> _003C_003E9__24_0;

		public static Action<PartyOrder> _003C_003E9__24_1;

		public static Func<PartyOrder, bool> _003C_003E9__24_2;

		public static Action<PartyOrder> _003C_003E9__24_3;

		public static Func<PartyOrder, bool> _003C_003E9__24_4;

		public static Action<PartyOrder> _003C_003E9__24_5;

		public static Func<PartyOrder, bool> _003C_003E9__24_6;

		public static Action<PartyOrder> _003C_003E9__24_7;

		public static Func<PartyOrder, bool> _003C_003E9__24_8;

		public static Action<PartyOrder> _003C_003E9__24_9;

		public static Func<Settlement, bool> _003C_003E9__27_0;

		public static Func<Settlement, string> _003C_003E9__27_1;

		internal void _003CRegisterDialogue_003Eb__23_8()
		{
			SetOrderResponse(CdeText.Get("{=cde.core.response.nevermind}As you wish."));
		}

		internal void _003CRegisterDialogue_003Eb__23_9()
		{
			PartyTransferScreen.Open(Hero.OneToOneConversationHero.PartyBelongedTo);
		}

		internal bool _003CRegisterDialogue_003Eb__23_10()
		{
			return !HasEligibleSettlement();
		}

		internal bool _003CRegisterDialogue_003Eb__23_11()
		{
			return !HasEligibleSettlement();
		}

		internal bool _003CRegisterDialogue_003Eb__23_16()
		{
			return HasMoreThanOnePage();
		}

		internal bool _003CRegisterDialogue_003Eb__23_17()
		{
			return HasMoreThanOnePage();
		}

		internal void _003CRegisterDialogue_003Eb__23_18()
		{
			SetOrderResponse(CdeText.Get("{=cde.core.response.wait}Of course. I will await your next orders."));
		}

		internal void _003CRegisterDialogue_003Eb__23_19()
		{
			SetOrderResponse(CdeText.Get("{=cde.core.response.wait}Of course. I will await your next orders."));
		}

		internal bool _003CRegisterRuleLines_003Eb__24_0(PartyOrder x)
		{
			return x.StopRecruitingTroops;
		}

		internal void _003CRegisterRuleLines_003Eb__24_1(PartyOrder x)
		{
			x.StopRecruitingTroops = !x.StopRecruitingTroops;
		}

		internal bool _003CRegisterRuleLines_003Eb__24_2(PartyOrder x)
		{
			return x.StopTakingPrisoners;
		}

		internal void _003CRegisterRuleLines_003Eb__24_3(PartyOrder x)
		{
			x.StopTakingPrisoners = !x.StopTakingPrisoners;
		}

		internal bool _003CRegisterRuleLines_003Eb__24_4(PartyOrder x)
		{
			return !x.AllowRaidingVillages;
		}

		internal void _003CRegisterRuleLines_003Eb__24_5(PartyOrder x)
		{
			x.AllowRaidingVillages = !x.AllowRaidingVillages;
		}

		internal bool _003CRegisterRuleLines_003Eb__24_6(PartyOrder x)
		{
			return !x.AllowSieges;
		}

		internal void _003CRegisterRuleLines_003Eb__24_7(PartyOrder x)
		{
			x.AllowSieges = !x.AllowSieges;
		}

		internal bool _003CRegisterRuleLines_003Eb__24_8(PartyOrder x)
		{
			return !x.AllowJoiningArmies;
		}

		internal void _003CRegisterRuleLines_003Eb__24_9(PartyOrder x)
		{
			x.AllowJoiningArmies = !x.AllowJoiningArmies;
		}

		internal bool _003CGetPlayerOwnedSettlements_003Eb__27_0(Settlement x)
		{
			if (x != null && x.IsActive && (x.IsTown || x.IsCastle))
			{
				return x.MapFaction == Hero.MainHero.MapFaction;
			}
			return false;
		}

		internal string _003CGetPlayerOwnedSettlements_003Eb__27_1(Settlement x)
		{
			return x.Name.ToString();
		}
	}

	private const int ResupplyDays = 7;

	private const int SettlementsPerPage = 6;

	private const string OrderDataKey = "CDE_CoreOrders";

	private const string OrderResponseVariable = "CDE_CORE_ORDER_RESPONSE";

	private Dictionary<Hero, PartyOrder> _orders = new Dictionary<Hero, PartyOrder>();

	private Dictionary<Hero, TroopRoster> _templates = new Dictionary<Hero, TroopRoster>();

	private Dictionary<Hero, TroopRoster> _templateRoots = new Dictionary<Hero, TroopRoster>();

	// These entries are transient.  They bridge the short interval between a
	// lord party being destroyed/released and the next safe campaign tick.
	private readonly Dictionary<Hero, Settlement> _pendingRespawns = new Dictionary<Hero, Settlement>();

	private int _patrolPage;

	private int _stayPage;

	private int _hideoutPage;

	public static CorePartyBehavior Instance { get; private set; }

	public CorePartyBehavior()
	{
		Instance = this;
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
		CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
		CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
		CampaignEvents.AfterSettlementEntered.AddNonSerializedListener(this, OnAfterSettlementEntered);
		CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, OnSettlementLeft);
		CampaignEvents.ConversationEnded.AddNonSerializedListener(this, OnConversationEnded);
		CampaignEvents.MapEventEnded.AddNonSerializedListener(this, OnMapEventEnded);
		CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnMobilePartyDestroyed);
		CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
	}

	public override void SyncData(IDataStore dataStore)
	{
		if (dataStore.IsLoading)
		{
			_orders = new Dictionary<Hero, PartyOrder>();
		}
		dataStore.SyncData("CDE_CoreOrders", ref _orders);
		dataStore.SyncData("CDE_RecruitmentTemplates", ref _templates);
		dataStore.SyncData("CDE_RecruitmentTemplateRoots", ref _templateRoots);
		if (_orders == null)
		{
			_orders = new Dictionary<Hero, PartyOrder>();
		}
		if (_templates == null)
		{
			_templates = new Dictionary<Hero, TroopRoster>();
		}
		if (_templateRoots == null)
		{
			_templateRoots = new Dictionary<Hero, TroopRoster>();
		}
		if (dataStore.IsLoading)
		{
			CleanupInvalidTemplates();
		}
	}

	public PartyOrder GetOrder(Hero hero)
	{
		if (hero == null || _orders == null)
		{
			return null;
		}
		_orders.TryGetValue(hero, out var value);
		return value;
	}

	public TroopRoster GetTemplate(Hero hero)
	{
		if (hero == null || _templates == null)
		{
			return null;
		}
		_templates.TryGetValue(hero, out var value);
		return value;
	}

	private void SaveCurrentRosterAsTemplate(Hero hero)
	{
		if (hero?.PartyBelongedTo != null)
		{
			TroopRoster troopRoster = new TroopRoster(null);
			troopRoster.Add(hero.PartyBelongedTo.MemberRoster);
			_templates[hero] = troopRoster;
			_templateRoots[hero] = troopRoster;
			SetOrderResponse(CdeText.Get("{=cde.core.template.response}Understood. I will use my current troop composition as my recruitment template."));
		}
	}

	public TroopRoster GetTemplateRoots(Hero hero)
	{
		if (hero == null || _templateRoots == null)
		{
			return null;
		}
		_templateRoots.TryGetValue(hero, out var value);
		return value;
	}

	private void OpenRecruitmentTemplateEditor(Hero hero)
	{
		if (hero != null && hero.PartyBelongedTo != null)
		{
			RecruitmentTemplateEditorScreen.Open(hero, GetTemplate(hero), GetTemplateRoots(hero), delegate(TroopRoster template, TroopRoster roots)
			{
				_templates[hero] = template;
				_templateRoots[hero] = roots;
			});
		}
	}

	private void SaveOtherHeroRosterAsTemplate(Hero targetHero, Hero sourceHero)
	{
		MobileParty mobileParty = sourceHero?.PartyBelongedTo;
		if (targetHero != null && IsEligibleTemplateSource(sourceHero) && mobileParty != null)
		{
			TroopRoster troopRoster = new TroopRoster(null);
			troopRoster.Add(mobileParty.MemberRoster);
			_templates[targetHero] = troopRoster;
			_templateRoots[targetHero] = troopRoster;
			SetOrderResponse(CdeText.Get("{=cde.core.template.other.response}Understood. I will use") + " " + sourceHero.Name?.ToString() + " " + CdeText.Get("{=cde.core.template.other.response.suffix}'s party as my recruitment template."));
		}
	}

	public void CancelOrder(Hero hero)
	{
		if (hero != null)
		{
			_orders.Remove(hero);
			if (hero.PartyBelongedTo != null)
			{
				hero.PartyBelongedTo.Ai.SetDoNotMakeNewDecisions(doNotMakeNewDecisions: false);
				hero.PartyBelongedTo.Ai.RethinkAtNextHourlyTick = true;
			}
		}
	}

	internal void ClearCompanionData(Hero hero)
	{
		if (hero == null)
		{
			return;
		}
		CancelOrder(hero);
		_templates?.Remove(hero);
		_templateRoots?.Remove(hero);
		_pendingRespawns.Remove(hero);
	}

	private void OnSessionLaunched(CampaignGameStarter starter)
	{
		CleanupInvalidTemplates();
		RepairIndependentPlayerArmy();
		RegisterDialogue(starter);
	}

	private void OnGameLoaded(CampaignGameStarter starter)
	{
		CleanupInvalidTemplates();
		RepairIndependentPlayerArmy();
	}

	private void CleanupInvalidTemplates()
	{
		if (_templates == null)
		{
			return;
		}
		foreach (Hero hero in _templates.Keys.ToList())
		{
			TroopRoster roster = _templates[hero];
			if (roster == null)
			{
				_templates.Remove(hero);
				_templateRoots?.Remove(hero);
				continue;
			}
			roster.RemoveIf(x => x.Character == null || !x.Character.IsInitialized || x.Character.IsHero);
			if (_templateRoots != null && _templateRoots.TryGetValue(hero, out TroopRoster roots) && roots != null)
			{
				roots.RemoveIf(x => x.Character == null || !x.Character.IsInitialized || x.Character.IsHero);
			}
		}
	}

	private void RepairIndependentPlayerArmy()
	{
		try
		{
			MobileParty mainParty = MobileParty.MainParty;
			Army army = mainParty?.Army;
			if (army == null || army.LeaderParty != mainParty || army.Kingdom != null)
			{
				return;
			}
			Traverse.Create(army).Method("OnAfterLoad").GetValue();
		}
		catch
		{
			// This private method is version dependent; leaving the native state
			// untouched is safer than failing campaign loading.
		}
	}

	private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
	{
		try
		{
			Hero hero = mobileParty?.LeaderHero;
			PartyOrder order = GetOrder(hero);
			if (hero == null || order == null || hero == Hero.MainHero || hero.Clan != Clan.PlayerClan || !hero.IsAlive || mobileParty.IsCaravan || mobileParty.IsGarrison || mobileParty.IsMilitia)
			{
				return;
			}
			_pendingRespawns[hero] = mobileParty.CurrentSettlement ?? hero.LastKnownClosestSettlement ?? hero.HomeSettlement;
		}
		catch
		{
			// A destroyed party can already be partially detached when this event fires.
		}
	}

	private void ProcessPendingRespawns()
	{
		if (_pendingRespawns.Count == 0 || Campaign.Current == null)
		{
			return;
		}
		foreach (KeyValuePair<Hero, Settlement> item in _pendingRespawns.ToList())
		{
			Hero hero = item.Key;
			if (hero == null || !hero.IsAlive || hero == Hero.MainHero || hero.Clan != Clan.PlayerClan || GetOrder(hero) == null)
			{
				_pendingRespawns.Remove(hero);
				continue;
			}
			if (hero.PartyBelongedTo != null || hero.PartyBelongedToAsPrisoner != null)
			{
				continue;
			}
			try
			{
				hero.ChangeState(Hero.CharacterStates.Active);
				Settlement settlement = item.Value ?? hero.LastKnownClosestSettlement ?? hero.HomeSettlement ?? MobileParty.MainParty.CurrentSettlement;
				if (settlement == null)
				{
					continue;
				}
				MobilePartyHelper.SpawnLordParty(hero, settlement);
				_pendingRespawns.Remove(hero);
				RestoreOrderMovement(hero.PartyBelongedTo);
			}
			catch
			{
				// Keep the marker for the next hourly tick if spawning is temporarily unavailable.
			}
		}
	}

	private void RegisterDialogue(CampaignGameStarter starter)
	{
		starter.AddPlayerLine("cde_core_open", "hero_main_options", "cde_core_menu", CdeText.Get("{=cde.core.dialogue.open}I have a new assignment for you."), IsValidClanParty, null, 96);
		starter.AddDialogLine("cde_core_menu", "cde_core_menu", "cde_core_options", CdeText.Get("{=cde.core.dialogue.what}What is it?"), null, null);
		starter.AddPlayerLine("cde_core_follow", "cde_core_options", "cde_core_order_response", CdeText.Get("{=cde.core.option.follow}Your party is to follow mine."), IsValidClanParty, delegate
		{
			SetFollow(Hero.OneToOneConversationHero);
		}, 105);
		starter.AddPlayerLine("cde_core_patrol_open", "cde_core_options", "cde_core_patrol_prompt", CdeText.Get("{=cde.core.option.patrol}I am sending you on patrol."), IsValidClanParty, delegate
		{
			_patrolPage = 0;
		}, 104);
		starter.AddPlayerLine("cde_core_stay_open", "cde_core_options", "cde_core_stay_prompt", CdeText.Get("{=cde.core.option.stay}Stay in a settlement until I give another order."), IsValidClanParty, delegate
		{
			_stayPage = 0;
		}, 103);
		starter.AddPlayerLine("cde_core_roam", "cde_core_options", "cde_core_order_response", CdeText.Get("{=cde.core.option.roam}I want you to roam the lands."), IsValidClanParty, delegate
		{
			SetRoam(Hero.OneToOneConversationHero);
		}, 102);
		starter.AddPlayerLine("cde_core_cancel", "cde_core_options", "cde_core_order_response", CdeText.Get("{=cde.core.option.cancel}Cancel all standing orders."), () => IsValidClanParty() && GetOrder(Hero.OneToOneConversationHero) != null, delegate
		{
			CancelOrderWithResponse(Hero.OneToOneConversationHero);
		}, 102);
		starter.AddPlayerLine("cde_core_nevermind", "cde_core_options", "lord_pretalk", CdeText.Get("{=cde.core.option.nevermind}Never mind."), null, null, 1);
		starter.AddPlayerLine("cde_core_rules", "cde_core_options", "cde_core_rules_menu", CdeText.Get("{=cde.core.option.rules}I have standing rules for your party."), IsValidClanParty, delegate
		{
			EnsureRulesOrder(Hero.OneToOneConversationHero);
		}, 101);
		starter.AddPlayerLine("cde_core_disband", "cde_core_options", "cde_core_disband_prompt", CdeText.Get("{=cde.core.option.disband}I want you and your entire party to merge into mine."), IsValidClanParty, null, 101);
		starter.AddDialogLine("cde_core_disband_prompt_line", "cde_core_disband_prompt", "cde_core_disband_list", CdeText.Get("{=cde.core.dialogue.confirm}Are you sure?"), null, null);
		starter.AddPlayerLine("cde_core_disband_confirm", "cde_core_disband_list", "close_window", CdeText.Get("{=cde.core.option.confirm}Yes, I am sure."), IsValidClanParty, delegate
		{
			MergeDisbandParty(Hero.OneToOneConversationHero.PartyBelongedTo, MobileParty.MainParty.Party);
		});
		starter.AddPlayerLine("cde_core_disband_cancel", "cde_core_disband_list", "lord_pretalk", CdeText.Get("{=cde.core.option.nevermind}Never mind."), null, null, 1);
		starter.AddPlayerLine("cde_core_army_join", "cde_core_options", "cde_core_army_join_prompt", CdeText.Get("{=cde.core.option.army}I want your party in my army."), IsValidClanParty, null);
		starter.AddDialogLine("cde_core_army_join_prompt_line", "cde_core_army_join_prompt", "cde_core_army_join_list", CdeText.Get("{=cde.core.dialogue.confirm}Are you sure?"), null, null);
		starter.AddPlayerLine("cde_core_army_join_confirm", "cde_core_army_join_list", "close_window", CdeText.Get("{=cde.core.option.confirm}Yes, I am sure."), IsValidClanParty, delegate
		{
			JoinPlayerArmy(Hero.OneToOneConversationHero);
		});
		starter.AddPlayerLine("cde_core_army_join_cancel", "cde_core_army_join_list", "lord_pretalk", CdeText.Get("{=cde.core.option.nevermind}Never mind."), null, null, 1);
		starter.AddPlayerLine("cde_core_cancel_all", "cde_core_options", "cde_core_cancel_all_prompt", CdeText.Get("{=cde.core.option.cancel.all}Spread the word, everyone's orders are hereby rescinded."), IsValidClanParty, null, 99);
		starter.AddDialogLine("cde_core_cancel_all_prompt_line", "cde_core_cancel_all_prompt", "cde_core_cancel_all_list", CdeText.Get("{=cde.core.dialogue.confirm}Are you sure?"), null, null);
		starter.AddPlayerLine("cde_core_cancel_all_confirm", "cde_core_cancel_all_list", "lord_pretalk", CdeText.Get("{=cde.core.option.confirm}Yes, I am sure."), null, delegate
		{
			CancelAllOrders();
		});
		starter.AddPlayerLine("cde_core_cancel_all_cancel", "cde_core_cancel_all_list", "lord_pretalk", CdeText.Get("{=cde.core.option.nevermind}Never mind."), null, null, 1);
		starter.AddPlayerLine("cde_core_caravan_equipment", "caravan_talk", "lord_pretalk", CdeText.Get("{=cde.core.option.equipment}Let me see your goods and equipment."), IsValidClanPartyOrCaravan, delegate
		{
			PartyEquipmentScreen.Open(Hero.OneToOneConversationHero.PartyBelongedTo);
		}, 101);
		starter.AddPlayerLine("cde_core_caravan_transfer_troops", "caravan_talk", "lord_pretalk", CdeText.Get("{=cde.core.option.transfer.troops}Let us exchange troops and prisoners."), IsValidClanCaravan, delegate
		{
			OpenClanCaravanTransfer();
		}, 102);
		// Caravan leaders encountered in settlements start in hero_main_options;
		// this distinct entry point avoids a lord_pretalk self-loop after the screen closes.
		starter.AddPlayerLine("cde_core_caravan_transfer_troops_main", "hero_main_options", "lord_pretalk", CdeText.Get("{=cde.core.option.transfer.troops}Let us exchange troops and prisoners."), IsValidClanCaravan, delegate
		{
			OpenClanCaravanTransfer();
		}, 102);
		starter.AddDialogLine("cde_core_rules_line", "cde_core_rules_menu", "cde_core_rules_list", CdeText.Get("{=cde.core.dialogue.rules}What rules should I follow?"), null, null);
		starter.AddPlayerLine("cde_core_template_view", "cde_core_rules_list", "cde_core_order_response", CdeText.Get("{=cde.core.option.template.view}Show me your recruitment plan."), IsValidClanParty, delegate
		{
			ShowRecruitmentTemplate(Hero.OneToOneConversationHero);
		}, 106);
		starter.AddPlayerLine("cde_core_template", "cde_core_rules_list", "cde_core_order_response", CdeText.Get("{=cde.core.option.template.party}Use your current troops as your recruitment template."), IsValidClanParty, delegate
		{
			SaveCurrentRosterAsTemplate(Hero.OneToOneConversationHero);
		}, 105);
		starter.AddPlayerLine("cde_core_template_player", "cde_core_rules_list", "cde_core_order_response", CdeText.Get("{=cde.core.option.template.player}Use my party's current troops as your recruitment template."), IsValidClanParty, delegate
		{
			SavePlayerRosterAsTemplate(Hero.OneToOneConversationHero);
		}, 104);
		starter.AddPlayerLine("cde_core_template_edit", "cde_core_rules_list", "lord_pretalk", CdeText.Get("{=cde.core.option.template.edit}Edit recruitment template."), IsValidClanParty, delegate
		{
			OpenRecruitmentTemplateEditor(Hero.OneToOneConversationHero);
		}, 103);
		starter.AddPlayerLine("cde_core_template_other", "cde_core_rules_list", "cde_core_template_other_prompt", CdeText.Get("{=cde.core.option.template.other}Use another hero's party as your recruitment template."), IsValidClanParty, null, 102);
		starter.AddPlayerLine("cde_core_template_clear", "cde_core_rules_list", "cde_core_order_response", CdeText.Get("{=cde.core.option.template.clear}Recruit troops without a composition template."), () => IsValidClanParty() && GetTemplate(Hero.OneToOneConversationHero) != null, delegate
		{
			ClearRecruitmentTemplate(Hero.OneToOneConversationHero);
		}, 101);
		starter.AddDialogLine("cde_core_template_other_prompt", "cde_core_template_other_prompt", "cde_core_template_other_list", CdeText.Get("{=cde.core.dialogue.template.other}Whose party should I use as the recruitment template?"), HasOtherTemplateSource, null);
		starter.AddDialogLine("cde_core_template_other_empty", "cde_core_template_other_prompt", "cde_core_template_other_list", CdeText.Get("{=cde.core.dialogue.template.empty}There are no other clan hero parties available to use as a recruitment template."), () => !HasOtherTemplateSource(), null);
		foreach (Hero item in (IEnumerable<Hero>)Hero.AllAliveHeroes)
		{
			Hero sourceHero = item;
			starter.AddPlayerLine("cde_core_template_other_" + sourceHero.StringId, "cde_core_template_other_list", "cde_core_order_response", sourceHero.Name.ToString(), () => IsValidClanParty() && IsEligibleTemplateSource(sourceHero), delegate
			{
				SaveOtherHeroRosterAsTemplate(Hero.OneToOneConversationHero, sourceHero);
			});
		}
		starter.AddPlayerLine("cde_core_template_other_back", "cde_core_template_other_list", "cde_core_rules_menu", CdeText.Get("{=cde.core.option.done}That is all."), null, null, 1);
		RegisterRuleLines(starter);
		object obj3 = _003C_003EO._003C0_003E__HasEligibleSettlement;
		if (obj3 == null)
		{
			object obj4 = new ConversationSentence.OnConditionDelegate(HasEligibleSettlement);
			_003C_003EO._003C0_003E__HasEligibleSettlement = (ConversationSentence.OnConditionDelegate)obj4;
			obj3 = obj4;
		}
		starter.AddDialogLine("cde_core_patrol_prompt_line", "cde_core_patrol_prompt", "cde_core_patrol_list", CdeText.Get("{=cde.core.dialogue.patrol.where}Which settlement should I patrol?"), (ConversationSentence.OnConditionDelegate)obj3, null);
		object obj5 = LegacyC._003C_003E9__20_8;
		if (obj5 == null)
		{
			object obj6 = (ConversationSentence.OnConditionDelegate)(() => !HasEligibleSettlement());
			LegacyC._003C_003E9__20_8 = (ConversationSentence.OnConditionDelegate)obj6;
			obj5 = obj6;
		}
		starter.AddDialogLine("cde_core_patrol_empty_line", "cde_core_patrol_prompt", "cde_core_patrol_list", CdeText.Get("{=cde.core.dialogue.patrol.empty}Your faction does not currently control any towns or castles that I can patrol."), (ConversationSentence.OnConditionDelegate)obj5, null);
		object obj7 = _003C_003EO._003C0_003E__HasEligibleSettlement;
		if (obj7 == null)
		{
			object obj8 = new ConversationSentence.OnConditionDelegate(HasEligibleSettlement);
			_003C_003EO._003C0_003E__HasEligibleSettlement = (ConversationSentence.OnConditionDelegate)obj8;
			obj7 = obj8;
		}
		starter.AddDialogLine("cde_core_stay_prompt_line", "cde_core_stay_prompt", "cde_core_stay_list", CdeText.Get("{=cde.core.dialogue.stay.where}Which settlement should I stay in?"), (ConversationSentence.OnConditionDelegate)obj7, null);
		object obj9 = LegacyC._003C_003E9__20_9;
		if (obj9 == null)
		{
			object obj10 = (ConversationSentence.OnConditionDelegate)(() => !HasEligibleSettlement());
			LegacyC._003C_003E9__20_9 = (ConversationSentence.OnConditionDelegate)obj10;
			obj9 = obj10;
		}
		starter.AddDialogLine("cde_core_stay_empty_line", "cde_core_stay_prompt", "cde_core_stay_list", CdeText.Get("{=cde.core.dialogue.stay.empty}Your faction does not currently control any towns or castles where I can stay."), (ConversationSentence.OnConditionDelegate)obj9, null);
		starter.AddDialogLine("cde_core_clear_hideout_prompt_line", "cde_core_clear_hideout_prompt", "cde_core_clear_hideout_list", CdeText.Get("{=cde.core.dialogue.clear.hideout.where}Which settlement should I search around?"), HasEligibleSettlement, null);
		starter.AddDialogLine("cde_core_clear_hideout_empty_line", "cde_core_clear_hideout_prompt", "cde_core_clear_hideout_list", CdeText.Get("{=cde.core.dialogue.clear.hideout.empty}Your faction does not currently control any towns or castles to search around."), () => !HasEligibleSettlement(), null);
		starter.AddDialogLine("cde_core_order_response_line", "cde_core_order_response", "cde_core_order_response_player", "{CDE_CORE_ORDER_RESPONSE}", null, null);
		starter.AddPlayerLine("cde_core_order_response_ack", "cde_core_order_response_player", "lord_pretalk", CdeText.Get("{=cde.core.dialogue.ack}Understood."), null, null);
		foreach (Settlement item2 in (List<Settlement>)Campaign.Current.Settlements)
		{
			Settlement selected = item2;
			starter.AddPlayerLine("cde_core_patrol_" + selected.StringId, "cde_core_patrol_list", "cde_core_order_response", selected.Name.ToString(), () => IsSettlementOnPage(selected, _patrolPage), delegate
			{
				SetPatrol(Hero.OneToOneConversationHero, selected);
			});
			starter.AddPlayerLine("cde_core_stay_" + selected.StringId, "cde_core_stay_list", "cde_core_order_response", selected.Name.ToString(), () => IsSettlementOnPage(selected, _stayPage), delegate
			{
				SetStay(Hero.OneToOneConversationHero, selected);
			});
			starter.AddPlayerLine("cde_core_clear_hideout_" + selected.StringId, "cde_core_clear_hideout_list", "cde_core_order_response", selected.Name.ToString(), () => IsSettlementOnPage(selected, _hideoutPage), delegate
			{
				SetClearHideoutNearSettlement(Hero.OneToOneConversationHero, selected);
			});
		}
		object obj11 = LegacyC._003C_003E9__20_10;
		if (obj11 == null)
		{
			object obj12 = (ConversationSentence.OnConditionDelegate)(() => HasMoreThanOnePage());
			LegacyC._003C_003E9__20_10 = (ConversationSentence.OnConditionDelegate)obj12;
			obj11 = obj12;
		}
		starter.AddPlayerLine("cde_core_patrol_more", "cde_core_patrol_list", "cde_core_patrol_prompt", CdeText.Get("{=cde.core.option.more}Show another group."), (ConversationSentence.OnConditionDelegate)obj11, delegate
		{
			_patrolPage = GetNextPage(_patrolPage);
		}, 10);
		object obj13 = LegacyC._003C_003E9__20_12;
		if (obj13 == null)
		{
			object obj14 = (ConversationSentence.OnConditionDelegate)(() => HasMoreThanOnePage());
			LegacyC._003C_003E9__20_12 = (ConversationSentence.OnConditionDelegate)obj14;
			obj13 = obj14;
		}
		starter.AddPlayerLine("cde_core_stay_more", "cde_core_stay_list", "cde_core_stay_prompt", CdeText.Get("{=cde.core.option.more}Show another group."), (ConversationSentence.OnConditionDelegate)obj13, delegate
		{
			_stayPage = GetNextPage(_stayPage);
		}, 10);
		starter.AddPlayerLine("cde_core_clear_hideout_more", "cde_core_clear_hideout_list", "cde_core_clear_hideout_prompt", CdeText.Get("{=cde.core.option.more}Show another group."), HasMoreThanOnePage, delegate
		{
			_hideoutPage = GetNextPage(_hideoutPage);
		}, 10);
		starter.AddPlayerLine("cde_core_patrol_cancel", "cde_core_patrol_list", "lord_pretalk", CdeText.Get("{=cde.core.option.nevermind}Never mind."), IsValidClanParty, null, 1);
		starter.AddPlayerLine("cde_core_stay_cancel", "cde_core_stay_list", "lord_pretalk", CdeText.Get("{=cde.core.option.nevermind}Never mind."), IsValidClanParty, null, 1);
		starter.AddPlayerLine("cde_core_clear_hideout_cancel", "cde_core_clear_hideout_list", "lord_pretalk", CdeText.Get("{=cde.core.option.nevermind}Never mind."), IsValidClanParty, null, 1);
	}

	private void RegisterRuleLines(CampaignGameStarter starter)
	{
		AddRuleLine(starter, "recruit", CdeText.Get("{=cde.core.rule.recruit.on}Do not recruit new troops."), CdeText.Get("{=cde.core.rule.recruit.off}You may recruit troops normally."), (PartyOrder x) => x.StopRecruitingTroops, delegate(PartyOrder x)
		{
			x.StopRecruitingTroops = !x.StopRecruitingTroops;
		});
		AddRuleLine(starter, "prisoners", CdeText.Get("{=cde.core.rule.prisoners.on}Do not take common prisoners."), CdeText.Get("{=cde.core.rule.prisoners.off}You may take prisoners normally."), (PartyOrder x) => x.StopTakingPrisoners, delegate(PartyOrder x)
		{
			x.StopTakingPrisoners = !x.StopTakingPrisoners;
		});
		AddRuleLine(starter, "raids", CdeText.Get("{=cde.core.rule.raids.on}Do not raid villages."), CdeText.Get("{=cde.core.rule.raids.off}You may raid enemy villages."), (PartyOrder x) => !x.AllowRaidingVillages, delegate(PartyOrder x)
		{
			x.AllowRaidingVillages = !x.AllowRaidingVillages;
		});
		AddRuleLine(starter, "sieges", CdeText.Get("{=cde.core.rule.sieges.on}Do not besiege towns or castles."), CdeText.Get("{=cde.core.rule.sieges.off}You may besiege enemy settlements."), (PartyOrder x) => !x.AllowSieges, delegate(PartyOrder x)
		{
			x.AllowSieges = !x.AllowSieges;
		});
		AddRuleLine(starter, "armies", CdeText.Get("{=cde.core.rule.armies.on}Do not join armies."), CdeText.Get("{=cde.core.rule.armies.off}You may join armies."), (PartyOrder x) => !x.AllowJoiningArmies, delegate(PartyOrder x)
		{
			x.AllowJoiningArmies = !x.AllowJoiningArmies;
		});
		AddRuleLine(starter, "garrisons", CdeText.Get("{=cde.core.rule.garrisons.on}Do not donate troops to other clans' garrisons."), CdeText.Get("{=cde.core.rule.garrisons.off}You may donate troops to other clans' garrisons."), (PartyOrder x) => !x.AllowDonatingToOtherClanGarrisons, delegate(PartyOrder x)
		{
			x.AllowDonatingToOtherClanGarrisons = !x.AllowDonatingToOtherClanGarrisons;
		});
		AddRuleLine(starter, "hideouts", CdeText.Get("{=cde.core.rule.hideouts.on}Do not clear hideouts while patrolling."), CdeText.Get("{=cde.core.rule.hideouts.off}Clear hideouts discovered while patrolling."), (PartyOrder x) => !x.AllowClearingHideouts, delegate(PartyOrder x)
		{
			x.AllowClearingHideouts = !x.AllowClearingHideouts;
		});
		starter.AddPlayerLine("cde_core_rules_back", "cde_core_rules_list", "lord_pretalk", CdeText.Get("{=cde.core.option.done}That is all."), null, null, 1);
	}

	private void AddRuleLine(CampaignGameStarter starter, string key, string enableText, string disableText, Func<PartyOrder, bool> enabled, Action<PartyOrder> toggle)
	{
		starter.AddPlayerLine("cde_core_rule_" + key, "cde_core_rules_list", "cde_core_rules_menu", enableText, delegate
		{
			PartyOrder order = GetOrder(Hero.OneToOneConversationHero);
			return IsValidClanParty() && order != null && !enabled(order);
		}, delegate
		{
			PartyOrder order = GetOrder(Hero.OneToOneConversationHero);
			if (order != null)
			{
				toggle(order);
				ApplyRuleChangeImmediately(order);
			}
		});
		starter.AddPlayerLine("cde_core_rule_" + key + "_off", "cde_core_rules_list", "cde_core_rules_menu", disableText, delegate
		{
			PartyOrder order = GetOrder(Hero.OneToOneConversationHero);
			return IsValidClanParty() && order != null && enabled(order);
		}, delegate
		{
			PartyOrder order = GetOrder(Hero.OneToOneConversationHero);
			if (order != null)
			{
				toggle(order);
				ApplyRuleChangeImmediately(order);
			}
		});
	}

	private static void ApplyRuleChangeImmediately(PartyOrder order)
	{
		MobileParty party = order?.Owner?.PartyBelongedTo;
		if (party == null)
		{
			return;
		}
		bool resetMovement = order.Type == CoreOrderType.RulesOnly;
		if (resetMovement)
		{
			// Rules-only orders must not inherit a stale escort or raid target.
			party.SetMoveModeHold();
		}
		bool blocked = EnforceRestrictions(party, order);
		if (resetMovement || blocked)
		{
			party.Ai.SetDoNotMakeNewDecisions(false);
			party.Ai.RethinkAtNextHourlyTick = true;
		}
	}

	private void SavePlayerRosterAsTemplate(Hero hero)
	{
		if (hero != null && hero.PartyBelongedTo != null && MobileParty.MainParty != null)
		{
			TroopRoster troopRoster = new TroopRoster(null);
			troopRoster.Add(MobileParty.MainParty.MemberRoster);
			_templates[hero] = troopRoster;
			_templateRoots[hero] = troopRoster;
			SetOrderResponse(CdeText.Get("{=cde.core.template.player.response}Understood. I will use your party's current troop composition as my recruitment template."));
		}
	}

	private void ClearRecruitmentTemplate(Hero hero)
	{
		if (hero != null && _templates.Remove(hero))
		{
			_templateRoots.Remove(hero);
			SetOrderResponse(CdeText.Get("{=cde.core.template.clear.response}Understood. I will recruit troops without a composition template."));
		}
	}

	private void EnsureRulesOrder(Hero hero)
	{
		if (hero == null || hero.PartyBelongedTo == null)
		{
			return;
		}
		PartyOrder order = GetOrder(hero);
		if (order == null)
		{
			order = PartyOrder.RulesOnly(hero);
			_orders[hero] = order;
		}
		if (order.Type == CoreOrderType.RulesOnly)
		{
			ApplyRuleChangeImmediately(order);
		}
	}

	private void ShowRecruitmentTemplate(Hero hero)
	{
		TroopRoster template = GetTemplate(hero);
		List<string> entries = template?.GetTroopRoster()?.Where(x => x.Character != null && x.Number > 0).OrderBy(x => x.Character.Name.ToString()).Select(x => x.Character.Name + " x" + x.Number).ToList();
		if (entries == null || entries.Count == 0)
		{
			SetOrderResponse(CdeText.Get("{=cde.core.template.view.response.none}There is no recruitment template set for this party."));
			return;
		}
		SetOrderResponse(CdeText.Get("{=cde.core.template.view.response.header}The current recruitment template is:") + " " + string.Join(", ", entries));
	}

	private bool IsValidClanParty()
	{
		Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
		if (oneToOneConversationHero != null && oneToOneConversationHero != Hero.MainHero && oneToOneConversationHero.Clan == Hero.MainHero.Clan && oneToOneConversationHero.PartyBelongedTo != null && oneToOneConversationHero.PartyBelongedTo != MobileParty.MainParty)
		{
			return !oneToOneConversationHero.PartyBelongedTo.IsCaravan;
		}
		return false;
	}

	private bool IsValidClanPartyOrCaravan()
	{
		Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
		if (oneToOneConversationHero != null && oneToOneConversationHero != Hero.MainHero && oneToOneConversationHero.Clan == Hero.MainHero.Clan && oneToOneConversationHero.PartyBelongedTo != null)
		{
			return oneToOneConversationHero.PartyBelongedTo != MobileParty.MainParty;
		}
		return false;
	}

	private bool IsValidClanCaravan()
	{
		Hero hero = Hero.OneToOneConversationHero;
		MobileParty party = hero?.PartyBelongedTo;
		if (hero == null || hero == Hero.MainHero || party == null || !party.IsActive || !party.IsCaravan || Hero.MainHero?.Clan == null)
		{
			return false;
		}
		CaravanPartyComponent caravan = party.CaravanPartyComponent;
		Hero owner = caravan?.Owner ?? caravan?.PartyOwner ?? party.Owner;
		return (owner != null && owner.Clan == Hero.MainHero.Clan) || party.ActualClan == Hero.MainHero.Clan || hero.Clan == Hero.MainHero.Clan;
	}

	private void OpenClanCaravanTransfer()
	{
		if (IsValidClanCaravan())
		{
			PartyTransferScreen.Open(Hero.OneToOneConversationHero.PartyBelongedTo);
		}
	}

	private static bool HasOtherTemplateSource()
	{
		return Hero.AllAliveHeroes.Any(IsEligibleTemplateSource);
	}

	private static bool IsEligibleTemplateSource(Hero hero)
	{
		Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
		MobileParty mobileParty = hero?.PartyBelongedTo;
		if (hero != null && hero != Hero.MainHero && hero != oneToOneConversationHero && hero.Clan == Hero.MainHero.Clan && mobileParty != null && mobileParty.IsActive && mobileParty.IsLordParty && mobileParty.LeaderHero == hero)
		{
			return !mobileParty.IsCaravan;
		}
		return false;
	}

	private static List<Settlement> GetPlayerOwnedSettlements()
	{
		if (Hero.MainHero?.Clan == null || Campaign.Current == null)
		{
			return new List<Settlement>();
		}
		return (from x in Campaign.Current.Settlements
			where x != null && x.IsActive && (x.IsTown || x.IsCastle) && x.MapFaction == Hero.MainHero.MapFaction
			orderby x.Name.ToString()
			select x).ToList();
	}

	private static bool HasEligibleSettlement()
	{
		return GetPlayerOwnedSettlements().Count > 0;
	}

	private bool IsSettlementOnPage(Settlement settlement, int page)
	{
		if (!IsValidClanParty())
		{
			return false;
		}
		int num = GetPlayerOwnedSettlements().IndexOf(settlement);
		if (num >= page * 6)
		{
			return num < (page + 1) * 6;
		}
		return false;
	}

	private static bool HasMoreThanOnePage()
	{
		return GetPlayerOwnedSettlements().Count > 6;
	}

	private static int GetNextPage(int currentPage)
	{
		int num = (GetPlayerOwnedSettlements().Count + 6 - 1) / 6;
		if (num <= 0)
		{
			return 0;
		}
		return (currentPage + 1) % num;
	}

	private void SetFollow(Hero hero)
	{
		if (hero?.PartyBelongedTo != null)
		{
			ApplyOrder(PartyOrder.Follow(hero, MobileParty.MainParty));
			SetOrderResponse(CdeText.Get("{=cde.core.response.follow}Certainly. I will follow your party from now on."));
		}
	}

	private void SetRoam(Hero hero)
	{
		if (hero?.PartyBelongedTo != null)
		{
			ApplyOrder(PartyOrder.Roam(hero));
			SetOrderResponse(CdeText.Get("{=cde.core.response.roam}Understood. I will resume my usual duties."));
		}
	}

	private void SetPatrol(Hero hero, Settlement settlement)
	{
		if (hero?.PartyBelongedTo != null && settlement != null)
		{
			ApplyOrder(PartyOrder.Patrol(hero, settlement));
			MBTextManager.SetTextVariable("CDE_CORE_SETTLEMENT", settlement.Name);
			SetOrderResponse(CdeText.Get("{=cde.core.response.patrol}Understood. I will patrol around {CDE_CORE_SETTLEMENT}."));
		}
	}

	private void SetStay(Hero hero, Settlement settlement)
	{
		if (hero?.PartyBelongedTo != null && settlement != null)
		{
			ApplyOrder(PartyOrder.Stay(hero, settlement));
			MBTextManager.SetTextVariable("CDE_CORE_SETTLEMENT", settlement.Name);
			SetOrderResponse(CdeText.Get("{=cde.core.response.stay}Understood. I will remain at {CDE_CORE_SETTLEMENT} until you give another order."));
		}
	}

	private void CancelOrderWithResponse(Hero hero)
	{
		CancelOrder(hero);
		SetOrderResponse(CdeText.Get("{=cde.core.response.cancel}Understood. I will return to my usual duties."));
	}

	private static void SetOrderResponse(string response)
	{
		MBTextManager.SetTextVariable("CDE_CORE_ORDER_RESPONSE", response);
	}

	private void ApplyOrder(PartyOrder order)
	{
		_orders[order.Owner] = order;
		MobileParty partyBelongedTo = order.Owner.PartyBelongedTo;
		if (order.Type != CoreOrderType.RulesOnly && partyBelongedTo.Army != null)
		{
			partyBelongedTo.Army = null;
		}
		// Escort orders are kept by the command score and hourly movement refresh.
		// Locking the native AI here makes a nearby escort party non-interactable.
		partyBelongedTo.Ai.SetDoNotMakeNewDecisions(order.Type != CoreOrderType.FollowPlayer && order.Type != CoreOrderType.Roam && order.Type != CoreOrderType.RulesOnly);
		partyBelongedTo.Ai.RethinkAtNextHourlyTick = true;
		IssueMovement(order);
	}

	private void CancelAllOrders()
	{
		foreach (Hero item in _orders.Keys.ToList())
		{
			CancelOrder(item);
		}
	}

	private void MergeDisbandParty(MobileParty sourceParty, PartyBase targetParty)
	{
		if (sourceParty == null || targetParty == null || sourceParty == MobileParty.MainParty || sourceParty.Party == targetParty)
		{
			return;
		}
		CancelOrder(sourceParty.LeaderHero);
		PlayerEncounter.LeaveEncounter = true;
		targetParty.ItemRoster.Add(sourceParty.ItemRoster.AsEnumerable());
		foreach (TroopRosterElement item in sourceParty.PrisonRoster.GetTroopRoster().ToList())
		{
			if (item.Character.IsHero)
			{
				TakePrisonerAction.Apply(targetParty, item.Character.HeroObject);
			}
			else
			{
				targetParty.PrisonRoster.AddToCounts(item.Character, item.Number, insertAtFront: false, item.WoundedNumber, item.Xp);
			}
		}
		foreach (TroopRosterElement item2 in sourceParty.MemberRoster.GetTroopRoster().ToList())
		{
			sourceParty.MemberRoster.RemoveTroop(item2.Character, item2.Number, default(UniqueTroopDescriptor), item2.WoundedNumber);
			if (item2.Character.IsHero)
			{
				AddHeroToPartyAction.Apply(item2.Character.HeroObject, targetParty.MobileParty);
			}
			else
			{
				targetParty.MemberRoster.AddToCounts(item2.Character, item2.Number, insertAtFront: false, item2.WoundedNumber, item2.Xp);
			}
		}
		DestroyPartyAction.Apply(null, sourceParty);
	}

	private void JoinPlayerArmy(Hero hero)
	{
		MobileParty mobileParty = hero?.PartyBelongedTo;
		if (mobileParty == null || MobileParty.MainParty == null)
		{
			return;
		}
		if (MobileParty.MainParty.Army == null)
		{
			if (Clan.PlayerClan.IsUnderMercenaryService || Clan.PlayerClan.Kingdom == null)
			{
				CreatePlayerArmy(Hero.MainHero, Hero.MainHero.HomeSettlement, Army.ArmyTypes.Patrolling);
			}
			else
			{
				Clan.PlayerClan.Kingdom.CreateArmy(Hero.MainHero, Hero.MainHero.HomeSettlement, Army.ArmyTypes.Patrolling);
			}
		}
		Army army = MobileParty.MainParty.Army;
		if (army != null)
		{
			PlayerEncounter.LeaveEncounter = true;
			CancelOrder(hero);
			mobileParty.AttachedTo = null;
			mobileParty.Ai.SetDoNotMakeNewDecisions(false);
			mobileParty.Army = army;
			SetPartyAiAction.GetActionForEscortingParty(mobileParty, MobileParty.MainParty, MobileParty.NavigationType.Default, isFromPort: false, isTargetingPort: false);
			mobileParty.Ai.RethinkAtNextHourlyTick = true;
		}
	}

	private static void CreatePlayerArmy(Hero leader, Settlement target, Army.ArmyTypes type)
	{
		if (leader != null && leader.IsActive && leader.PartyBelongedTo != null)
		{
			Army army = new Army(Clan.PlayerClan.Kingdom, leader.PartyBelongedTo, type);
			army.Gather(target);
			CampaignEventDispatcher.Instance.OnArmyCreated(army);
			if (leader == Hero.MainHero)
			{
				(Game.Current?.GameStateManager?.ActiveState as MapState)?.OnArmyCreated(leader.PartyBelongedTo);
			}
		}
	}

	private void OnAfterSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
	{
		if (party == null || settlement == null)
		{
			return;
		}
		PartyOrder order = GetOrder(party.LeaderHero);
		if (order != null && (order.ResupplySettlement == settlement || NeedsFood(party)))
		{
			TryBuyFood(party, settlement);
			if (!NeedsFood(party))
			{
				order.EndResupply();
				IssueMovement(order);
			}
		}
		if (order != null)
		{
			ProcessSettlementInventory(party, settlement);
		}
		if (party != MobileParty.MainParty)
		{
			return;
		}
		foreach (PartyOrder item in _orders.Values.Where((PartyOrder x) => x != null && x.Type == CoreOrderType.FollowPlayer && x.TargetParty == party))
		{
			MobileParty mobileParty = item.Owner?.PartyBelongedTo;
			if (mobileParty != null && mobileParty.IsActive)
			{
				mobileParty.SetMoveGoToSettlement(settlement, MobileParty.NavigationType.Default, isTargetingThePort: false);
			}
		}
	}

	private void OnSettlementLeft(MobileParty party, Settlement settlement)
	{
		if (party != MobileParty.MainParty)
		{
			return;
		}
		foreach (PartyOrder item in _orders.Values.Where((PartyOrder x) => x != null && x.Type == CoreOrderType.FollowPlayer && x.TargetParty == party))
		{
			IssueMovement(item);
		}
	}

	private void OnHourlyTick()
	{
		ProcessPendingRespawns();
		foreach (KeyValuePair<Hero, PartyOrder> item in _orders.ToList())
		{
			PartyOrder value = item.Value;
			if (!IsOrderValid(value))
			{
				CancelOrder(item.Key);
				continue;
			}
			MobileParty partyBelongedTo = value.Owner.PartyBelongedTo;
			if (value.Type == CoreOrderType.ClearNearbyHideout && ProcessHideoutOrder(value, partyBelongedTo))
			{
				continue;
			}
			if (value.Type == CoreOrderType.StayInSettlement && value.TargetSettlement != null && value.Owner.PartyBelongedTo.CurrentSettlement == value.TargetSettlement && !NeedsFood(value.Owner.PartyBelongedTo))
			{
				value.Owner.PartyBelongedTo.SetMoveModeHold();
				continue;
			}
			if (value.ResupplySettlement != null)
			{
				if (value.ResupplySettlement.IsActive)
				{
					if (partyBelongedTo.CurrentSettlement == value.ResupplySettlement)
					{
						TryBuyFood(partyBelongedTo, value.ResupplySettlement);
						if (!NeedsFood(partyBelongedTo))
						{
							value.EndResupply();
							IssueMovement(value);
						}
						else
						{
							partyBelongedTo.SetMoveModeHold();
						}
					}
					else
					{
						partyBelongedTo.SetMoveGoToSettlement(value.ResupplySettlement, MobileParty.NavigationType.Default, isTargetingThePort: false);
					}
					continue;
				}
				value.EndResupply();
			}
			if (EnforceRestrictions(partyBelongedTo, value))
			{
				// A forbidden raid or siege must not fall through to IssueMovement.
				// Releasing the native decision lock lets the AI choose a legal
				// behavior on the next hourly tick instead of being stuck in escort.
				partyBelongedTo.Ai.SetDoNotMakeNewDecisions(false);
				partyBelongedTo.Ai.RethinkAtNextHourlyTick = true;
				continue;
			}
			if (value.Type == CoreOrderType.PatrolSettlement && value.AllowClearingHideouts && TryClearHideoutDuringPatrol(value, partyBelongedTo))
			{
				continue;
			}
			if (NeedsFood(partyBelongedTo))
			{
				Settlement settlement = FindFoodSettlement(partyBelongedTo);
				if (settlement != null)
				{
					value.BeginResupply(settlement);
					partyBelongedTo.SetMoveGoToSettlement(settlement, MobileParty.NavigationType.Default, isTargetingThePort: false);
					continue;
				}
			}
			IssueMovement(value);
		}
	}

	private static bool EnforceRestrictions(MobileParty party, PartyOrder order)
	{
		if (party == null || order == null)
		{
			return false;
		}
		bool blocked = false;
		if (party.Army != null && party.Army.LeaderParty != party)
		{
			bool isPlayerArmy = party.Army.LeaderParty == MobileParty.MainParty;
			// A rules-only order only changes long-term restrictions. It must not
			// remove a party that has already joined the player's army.
			if (!order.AllowJoiningArmies && !isPlayerArmy)
			{
				party.Army = null;
				party.AttachedTo = null;
				party.SetMoveModeHold();
				blocked = true;
			}
		}
		if (!order.AllowSieges && (party.BesiegedSettlement != null || party.DefaultBehavior == AiBehavior.BesiegeSettlement || party.DefaultBehavior == AiBehavior.AssaultSettlement))
		{
			party.SetMoveModeHold();
			blocked = true;
		}
		if (!order.AllowRaidingVillages && party.DefaultBehavior == AiBehavior.RaidSettlement)
		{
			party.SetMoveModeHold();
			blocked = true;
		}
		return blocked;
	}

	private void SetClearNearbyHideout(Hero hero)
	{
		MobileParty party = hero?.PartyBelongedTo;
		Settlement hideout = FindNearbyInfestedHideout(party);
		if (hero == null || party == null || hideout == null)
		{
			SetOrderResponse(CdeText.Get("{=cde.core.response.clear.hideout.none}There are no active hideouts near a town for me to clear."));
			return;
		}
		ApplyOrder(PartyOrder.ClearNearbyHideout(hero, hideout));
		MBTextManager.SetTextVariable("CDE_CORE_SETTLEMENT", hideout.Name);
		SetOrderResponse(CdeText.Get("{=cde.core.response.clear.hideout}Understood. I will clear the hideout near {CDE_CORE_SETTLEMENT}."));
	}

	private void SetClearHideoutNearSettlement(Hero hero, Settlement settlement)
	{
		Settlement hideout = FindInfestedHideoutNearSettlement(settlement);
		if (hero == null || hero.PartyBelongedTo == null || hideout == null)
		{
			MBTextManager.SetTextVariable("CDE_CORE_SETTLEMENT", settlement == null ? "" : settlement.Name.ToString());
			SetOrderResponse(CdeText.Get("{=cde.core.response.clear.hideout.specific.none}There are no active hideouts near {CDE_CORE_SETTLEMENT}."));
			return;
		}
		ApplyOrder(PartyOrder.ClearNearbyHideout(hero, hideout));
		MBTextManager.SetTextVariable("CDE_CORE_SETTLEMENT", settlement.Name);
		SetOrderResponse(CdeText.Get("{=cde.core.response.clear.hideout.specific}Understood. I will clear a hideout near {CDE_CORE_SETTLEMENT}."));
	}

	private static Settlement FindNearbyInfestedHideout(MobileParty party)
	{
		if (party == null || Campaign.Current == null)
		{
			return null;
		}
		return (from hideout in Hideout.All
			where hideout != null && hideout.IsInfested && hideout.Settlement != null && hideout.Settlement.IsActive
			let distance = party.GetPosition2D.Distance(hideout.Settlement.GetPosition2D)
			where GetPlayerOwnedSettlements().Any(settlement => settlement.GetPosition2D.Distance(hideout.Settlement.GetPosition2D) <= 35f)
			orderby distance
			select hideout.Settlement).FirstOrDefault();
	}

	private static Settlement FindInfestedHideoutNearSettlement(Settlement settlement)
	{
		if (settlement == null || !settlement.IsActive || (!settlement.IsTown && !settlement.IsCastle) || Campaign.Current == null)
		{
			return null;
		}
		return (from hideout in Hideout.All
			where hideout != null && hideout.IsInfested && hideout.Settlement != null && hideout.Settlement.IsActive
			where hideout.Settlement.GetPosition2D.Distance(settlement.GetPosition2D) <= 35f
			orderby hideout.Settlement.GetPosition2D.Distance(settlement.GetPosition2D)
			select hideout.Settlement).FirstOrDefault();
	}

	private static bool TryClearHideoutDuringPatrol(PartyOrder order, MobileParty party)
	{
		if (order?.TargetSettlement == null || party == null || !party.IsActive || party.MapEvent != null)
		{
			return false;
		}
		float detectionRange = Math.Max(5f, party.SeeingRange);
		Settlement hideout = (from candidate in Hideout.All
			where candidate != null && candidate.IsInfested && candidate.Settlement != null && candidate.Settlement.IsActive
			where candidate.Settlement.GetPosition2D.Distance(order.TargetSettlement.GetPosition2D) <= 35f
			where candidate.Settlement.GetPosition2D.Distance(party.GetPosition2D) <= detectionRange
			orderby candidate.Settlement.GetPosition2D.Distance(party.GetPosition2D)
			select candidate.Settlement).FirstOrDefault();
		if (hideout == null)
		{
			return false;
		}
		if (party.GetPosition2D.Distance(hideout.GetPosition2D) > 2.5f)
		{
			party.SetMoveGoToSettlement(hideout, MobileParty.NavigationType.Default, isTargetingThePort: false);
			return true;
		}
		try
		{
			Hideout component = hideout.SettlementComponent as Hideout;
			if (component == null)
			{
				return false;
			}
			int defenderIndex = 0;
			PartyBase defender = component.GetNextDefenderParty(ref defenderIndex, MapEvent.BattleTypes.Hideout);
			if (defender == null)
			{
				return false;
			}
			HideoutEventComponent.CreateHideoutEvent(party.Party, defender, false);
			return true;
		}
		catch
		{
			party.SetMovePatrolAroundSettlement(order.TargetSettlement, MobileParty.NavigationType.Default, isTargetingPort: false);
			return true;
		}
	}

	private static void OnDailyTick()
	{
		int minimumGold = Config.Value.ClanPartyGoldLimitToTakeFromTreasury;
		if (minimumGold <= 0 || Hero.MainHero == null || Clan.PlayerClan == null)
		{
			return;
		}
		foreach (WarPartyComponent component in Clan.PlayerClan.WarPartyComponents.ToList())
		{
			MobileParty party = component?.MobileParty;
			Hero leader = party?.LeaderHero;
			if (party == null || !party.IsActive || party.IsGarrison || party.IsMilitia || party.IsVillager || party.IsCaravan || party.IsMainParty || leader == null)
			{
				continue;
			}
			if (leader.Gold < minimumGold && Hero.MainHero.Gold > 0)
			{
				int amount = Math.Min(minimumGold - leader.Gold, Hero.MainHero.Gold);
				if (amount > 0)
				{
					GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, leader, amount, disableNotification: true);
				}
			}
		}
	}

	private void OnConversationEnded(IEnumerable<CharacterObject> characters)
	{
		CharacterObject character = characters?.FirstOrDefault();
		Hero hero = character?.HeroObject;
		PartyOrder order = GetOrder(hero);
		MobileParty party = hero?.PartyBelongedTo;
		if (order == null || party == null || !party.IsActive)
		{
			return;
		}
		if (order.Type == CoreOrderType.FollowPlayer && order.TargetParty != null && order.TargetParty.IsActive)
		{
			party.SetMoveEscortParty(order.TargetParty, MobileParty.NavigationType.Default, isTargetingPort: false);
		}
		party.Ai.RethinkAtNextHourlyTick = true;
	}

	private void OnMapEventEnded(MapEvent mapEvent)
	{
		if (mapEvent?.InvolvedParties == null)
		{
			return;
		}
		foreach (MobileParty party in mapEvent.InvolvedParties.Select(x => x?.MobileParty).Where(x => x != null).Distinct().ToList())
		{
			PartyOrder order = GetOrder(party.LeaderHero);
			if (order?.Type == CoreOrderType.ClearNearbyHideout && mapEvent.IsHideoutBattle)
			{
				Settlement next = FindNearbyInfestedHideout(party);
				if (next != null)
				{
					order.SetTargetSettlement(next);
					party.Ai.RethinkAtNextHourlyTick = true;
					IssueMovement(order);
				}
				else
				{
					CancelOrder(party.LeaderHero);
				}
			}
			else if (order?.Type == CoreOrderType.PatrolSettlement && mapEvent.IsHideoutBattle)
			{
				RestoreOrderMovement(party);
			}
			else if (order?.Type == CoreOrderType.FollowPlayer)
			{
				RestoreOrderMovement(party);
			}
		}
	}

	public void RestoreEscortOrders()
	{
		foreach (PartyOrder order in _orders.Values.Where(x => x != null && x.Type == CoreOrderType.FollowPlayer && x.TargetParty != null && x.TargetParty.IsActive).ToList())
		{
			MobileParty party = order.Owner?.PartyBelongedTo;
			if (party != null && party.IsActive && party.Army == null)
			{
				party.SetMoveEscortParty(order.TargetParty, MobileParty.NavigationType.Default, isTargetingPort: false);
			}
		}
	}

	public void RestoreOrderMovement(MobileParty party)
	{
		PartyOrder order = GetOrder(party?.LeaderHero);
		if (order == null || party == null || !party.IsActive || order.ResupplySettlement != null)
		{
			return;
		}
		IssueMovement(order);
	}

	public void CommandEscortAttack(MobileParty target)
	{
		if (target == null || !target.IsActive)
		{
			return;
		}
		foreach (PartyOrder order in _orders.Values.Where(x => x != null && x.Type == CoreOrderType.FollowPlayer && x.TargetParty == MobileParty.MainParty).ToList())
		{
			MobileParty escort = order.Owner?.PartyBelongedTo;
			if (escort == null || !escort.IsActive || escort.Army != null)
			{
				continue;
			}
			float distance = escort.GetPosition2D.Distance(target.GetPosition2D);
			if (distance > escort.SeeingRange)
			{
				continue;
			}
			if (FactionManager.IsAtWarAgainstFaction(escort.MapFaction, target.MapFaction))
			{
				escort.SetMoveEngageParty(target, MobileParty.NavigationType.Default);
			}
			else
			{
				escort.SetMoveEscortParty(MobileParty.MainParty, MobileParty.NavigationType.Default, isTargetingPort: false);
			}
		}
	}

	private static bool NeedsFood(MobileParty party)
	{
		if (party != null && party.FoodChange < 0f)
		{
			float triggerDays = Config.Value.ResupplyTriggerDays;
			if (float.IsNaN(triggerDays) || float.IsInfinity(triggerDays) || triggerDays < 0f)
			{
				triggerDays = 2f;
			}
			return party.GetNumDaysForFoodToLast() <= triggerDays;
		}
		return false;
	}

	private static Settlement FindFoodSettlement(MobileParty party)
	{
		return (from x in Campaign.Current.Settlements
			where x.IsActive && x.IsTown && x.Party != null && !FactionManager.IsAtWarAgainstFaction(party.MapFaction, x.MapFaction) && x.ItemRoster.TotalFood >= Math.Max(1, (int)Math.Ceiling((0f - party.FoodChange) * Math.Max(1f, Config.Value.ResupplyTargetDays / 2f)))
			orderby party.GetPosition2D.Distance(x.GetPosition2D)
			select x).FirstOrDefault();
	}

	private static void TryBuyFood(MobileParty party, Settlement settlement)
	{
		if (party?.LeaderHero == null || settlement?.Party == null || settlement.ItemRoster.TotalFood <= 0)
		{
			return;
		}
		float targetDays = Config.Value.ResupplyTargetDays;
		if (float.IsNaN(targetDays) || float.IsInfinity(targetDays) || targetDays < 1f)
		{
			targetDays = 7f;
		}
		int num = (int)Math.Ceiling(Math.Max(0.1f, 0f - party.FoodChange) * targetDays);
		int num2 = Math.Min(Math.Max(0, num - party.TotalFoodAtInventory), settlement.ItemRoster.TotalFood);
		for (int i = 0; i < num2; i++)
		{
			ItemRosterElement itemRosterElement = default(ItemRosterElement);
			float itemElementsPrice = 0f;
			Campaign.Current.Models.PartyFoodBuyingModel.FindItemToBuy(party, settlement, out itemRosterElement, out itemElementsPrice);
			if (itemRosterElement.EquipmentElement.Item != null && !(itemElementsPrice > (float)party.LeaderHero.Gold))
			{
				SellItemsAction.Apply(settlement.Party, party.Party, itemRosterElement, 1);
				if (itemRosterElement.EquipmentElement.Item.HasHorseComponent && itemRosterElement.EquipmentElement.Item.HorseComponent.IsLiveStock)
				{
					i += itemRosterElement.EquipmentElement.Item.HorseComponent.MeatCount - 1;
				}
				continue;
			}
			break;
		}
	}

	private static void ProcessSettlementInventory(MobileParty party, Settlement settlement)
	{
		if (party == null || settlement == null || !settlement.IsTown || party == MobileParty.MainParty || !party.IsLordParty || party.Party == null || party.LeaderHero == null)
		{
			return;
		}
		List<ItemRosterElement> items = party.ItemRoster.ToList();
		foreach (ItemRosterElement item in items)
		{
			ItemObject itemObject = item.EquipmentElement.Item;
			if (itemObject == null || itemObject.IsFood || itemObject.IsMountable)
			{
				continue;
			}
			if (item.Amount > 0)
			{
				SellItemsAction.Apply(party.Party, settlement.Party, item, item.Amount, settlement);
			}
		}

		int spare = Config.Value.SpareMountsToKeep;
		if (spare < 0)
		{
			spare = 0;
		}
		int mountsToKeep = Math.Max(0, party.Party.NumberOfRegularMembers - party.Party.NumberOfMenWithHorse) + spare;
		int excess = party.Party.NumberOfMounts - mountsToKeep;
		if (excess <= 0)
		{
			return;
		}
		while (excess > 0)
		{
			ItemRosterElement best = default(ItemRosterElement);
			int bestValue = -1;
			foreach (ItemRosterElement item in party.ItemRoster)
			{
				ItemObject itemObject = item.EquipmentElement.Item;
				if (itemObject != null && itemObject.IsMountable && item.Amount > 0 && itemObject.Value > bestValue)
				{
					best = item;
					bestValue = itemObject.Value;
				}
			}
			if (bestValue < 0)
			{
				break;
			}
			SellItemsAction.Apply(party.Party, settlement.Party, best, 1, settlement);
			excess--;
		}
	}

	private static bool IsOrderValid(PartyOrder order)
	{
		object obj = order?.Owner?.PartyBelongedTo;
		MobileParty mobileParty = (MobileParty)obj;
		if (mobileParty == null || !mobileParty.IsActive)
		{
			return false;
		}
		if (order.Type == CoreOrderType.FollowPlayer)
		{
			if (order.TargetParty != null)
			{
				return order.TargetParty.IsActive;
			}
			return false;
		}
		if (order.Type == CoreOrderType.RulesOnly || order.Type == CoreOrderType.Roam)
		{
			return true;
		}
		if (order.Type == CoreOrderType.ClearNearbyHideout)
		{
			Hideout hideout = order.TargetSettlement?.SettlementComponent as Hideout;
			return hideout != null && hideout.IsInfested && order.TargetSettlement.IsActive;
		}
		if (order.TargetSettlement != null && order.TargetSettlement.IsActive)
		{
			return order.TargetSettlement.MapFaction == Hero.MainHero.MapFaction;
		}
		return false;
	}

	private static void IssueMovement(PartyOrder order)
	{
		MobileParty mobileParty = order.Owner?.PartyBelongedTo;
		if (mobileParty == null || !mobileParty.IsActive)
		{
			return;
		}
		switch (order.Type)
		{
		case CoreOrderType.FollowPlayer:
			if (order.TargetParty != null && order.TargetParty.IsActive)
			{
				EnsureIndependentEscortParty(mobileParty);
				mobileParty.SetMoveEscortParty(order.TargetParty, MobileParty.NavigationType.Default, isTargetingPort: false);
			}
			break;
		case CoreOrderType.PatrolSettlement:
			if (order.TargetSettlement != null && order.TargetSettlement.IsActive)
			{
				mobileParty.SetMovePatrolAroundSettlement(order.TargetSettlement, MobileParty.NavigationType.Default, isTargetingPort: false);
			}
			break;
		case CoreOrderType.StayInSettlement:
			if (order.TargetSettlement != null && order.TargetSettlement.IsActive)
			{
				if (mobileParty.CurrentSettlement == order.TargetSettlement)
				{
					mobileParty.SetMoveModeHold();
				}
				else
				{
					mobileParty.SetMoveGoToSettlement(order.TargetSettlement, MobileParty.NavigationType.Default, isTargetingThePort: false);
				}
			}
			break;
		case CoreOrderType.ClearNearbyHideout:
			if (order.TargetSettlement != null && order.TargetSettlement.IsActive)
			{
				mobileParty.SetMoveGoToSettlement(order.TargetSettlement, MobileParty.NavigationType.Default, isTargetingThePort: false);
			}
			break;
		}
	}

	private bool ProcessHideoutOrder(PartyOrder order, MobileParty party)
	{
		if (order == null || party == null || !party.IsActive)
		{
			return true;
		}
		Hideout hideout = order.TargetSettlement?.SettlementComponent as Hideout;
		if (hideout == null || !hideout.IsInfested || !order.TargetSettlement.IsActive)
		{
			Settlement next = FindNearbyInfestedHideout(party);
			if (next == null)
			{
				CancelOrder(order.Owner);
				return true;
			}
			order.SetTargetSettlement(next);
			hideout = next.SettlementComponent as Hideout;
		}
		if (party.MapEvent != null)
		{
			return true;
		}
		if (party.GetPosition2D.Distance(order.TargetSettlement.GetPosition2D) > 2.5f)
		{
			party.SetMoveGoToSettlement(order.TargetSettlement, MobileParty.NavigationType.Default, isTargetingThePort: false);
			return true;
		}
		try
		{
			int defenderIndex = 0;
			PartyBase defender = hideout.GetNextDefenderParty(ref defenderIndex, MapEvent.BattleTypes.Hideout);
			if (defender == null)
			{
				return true;
			}
			HideoutEventComponent.CreateHideoutEvent(party.Party, defender, false);
		}
		catch
		{
			party.Ai.SetDoNotMakeNewDecisions(false);
			party.Ai.RethinkAtNextHourlyTick = true;
		}
		return true;
	}

	private static void EnsureIndependentEscortParty(MobileParty party)
	{
		if (party == null || party == MobileParty.MainParty)
		{
			return;
		}
		if (party.Army != null)
		{
			party.Army = null;
		}
		if (party.AttachedTo != null)
		{
			party.AttachedTo = null;
		}
		party.Ai.SetDoNotMakeNewDecisions(false);
	}
}
