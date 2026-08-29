using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace ClanDialogueExpansion;

internal static class RecruitmentTemplateEditorScreen
{
	private const int TemplateSizeLimit = 1000000;

	public static void Open(Hero targetHero, TroopRoster existingTemplate, TroopRoster existingRoots, Action<TroopRoster, TroopRoster> saveTemplate)
	{
		if (targetHero == null || targetHero.PartyBelongedTo == null || saveTemplate == null)
		{
			return;
		}
		try
		{
			TroopRoster leftMemberRoster = CreateAvailableTroopsRoster();
			TroopRoster troopRoster = new TroopRoster(null);
			if (existingTemplate != null)
			{
				troopRoster.Add(existingTemplate);
			}
			TroopRoster roots = new TroopRoster(null);
			if (existingRoots != null)
			{
				roots.Add(existingRoots);
			}
			else if (existingTemplate != null)
			{
				roots.Add(existingTemplate);
			}
			PartyScreenLogic logic = new PartyScreenLogic();
			PartyScreenLogicInitializationData initializationData = new PartyScreenLogicInitializationData
			{
				LeftMemberRoster = leftMemberRoster,
				LeftPrisonerRoster = new TroopRoster(null),
				RightMemberRoster = troopRoster,
				RightPrisonerRoster = new TroopRoster(null),
				LeftPartyName = new TextObject(CdeText.Get("{=cde.core.screen.template.available}Available Troop Types")),
				RightPartyName = new TextObject(targetHero.Name?.ToString() + " " + CdeText.Get("{=cde.core.screen.template.right}Recruitment Template")),
				Header = new TextObject(CdeText.Get("{=cde.core.screen.template.header}Recruitment Template")),
				LeftPartyMembersSizeLimit = 1000000,
				RightPartyMembersSizeLimit = 1000000,
				LeftPartyPrisonersSizeLimit = 0,
				RightPartyPrisonersSizeLimit = 0,
				TroopTransferableDelegate = CanTransferTroop,
				PartyPresentationDoneButtonDelegate = (TroopRoster leftMembers, TroopRoster leftPrisoners, TroopRoster rightMembers, TroopRoster rightPrisoners, FlattenedTroopRoster takenPrisoners, FlattenedTroopRoster releasedPrisoners, bool isForced, PartyBase leftParty, PartyBase rightParty) => true,
				PartyPresentationDoneButtonConditionDelegate = (TroopRoster leftMembers, TroopRoster leftPrisoners, TroopRoster rightMembers, TroopRoster rightPrisoners, int leftLimit, int rightLimit) => new Tuple<bool, TextObject>(item1: true, TextObject.GetEmpty()),
				DoNotApplyGoldTransactions = true,
				MemberTransferState = PartyScreenLogic.TransferState.Transferable,
				PrisonerTransferState = PartyScreenLogic.TransferState.NotTransferable,
				AccompanyingTransferState = PartyScreenLogic.TransferState.NotTransferable,
				IsTroopUpgradesDisabled = true,
				PartyScreenMode = PartyScreenHelper.PartyScreenMode.TroopsManage
			};
			logic.Initialize(initializationData);
			logic.Update += delegate(PartyScreenLogic.PartyCommand command)
			{
				if (command != null && command.Code == PartyScreenLogic.PartyCommandCode.TransferTroop && command.Type == PartyScreenLogic.TroopType.Member)
				{
					if (command.RosterSide == PartyScreenLogic.PartyRosterSide.Left)
					{
						roots.AddToCounts(command.Character, command.TotalNumber);
						ExpandTree(command.Character, logic.MemberRosters[1], logic.MemberRosters[0]);
					}
					else if (command.RosterSide == PartyScreenLogic.PartyRosterSide.Right)
					{
						roots.AddToCounts(command.Character, -command.TotalNumber);
					}
				}
			};
			logic.PartyPresentationDoneButtonDelegate = delegate(TroopRoster leftMembers, TroopRoster leftPrisoners, TroopRoster rightMembers, TroopRoster rightPrisoners, FlattenedTroopRoster takenPrisoners, FlattenedTroopRoster releasedPrisoners, bool isForced, PartyBase leftParty, PartyBase rightParty)
			{
				TroopRoster troopRoster2 = new TroopRoster(null);
				foreach (TroopRosterElement item in from element in rightMembers.GetTroopRoster()
					where element.Number > 0
					select element)
				{
					troopRoster2.AddToCounts(item.Character, item.Number, insertAtFront: false, item.WoundedNumber, item.Xp);
				}
				TroopRoster troopRoster3 = new TroopRoster(null);
				foreach (TroopRosterElement item2 in from element in roots.GetTroopRoster()
					where element.Number > 0
					select element)
				{
					troopRoster3.AddToCounts(item2.Character, rightMembers.GetTroopCount(item2.Character));
				}
				saveTemplate(troopRoster2, troopRoster3);
				return true;
			};
			PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
			partyState.PartyScreenLogic = logic;
			partyState.PartyScreenMode = PartyScreenHelper.PartyScreenMode.TroopsManage;
			partyState.IsDonating = false;
			Game.Current.GameStateManager.PushState(partyState);
		}
		catch (Exception ex)
		{
			InformationManager.DisplayMessage(new InformationMessage(CdeText.Get("{=cde.core.error.template}Recruitment template screen could not be opened:") + " " + ex.Message));
		}
	}

	private static TroopRoster CreateAvailableTroopsRoster()
	{
		TroopRoster troopRoster = new TroopRoster(null);
		foreach (CharacterObject item in from character in CharacterObject.FindAll(IsRecruitableTroop)
			orderby character.Name.ToString()
			select character)
		{
			troopRoster.AddToCounts(item, 1000);
		}
		return troopRoster;
	}

	private static bool IsRecruitableTroop(CharacterObject character)
	{
		if (!IsPotentialTroop(character))
		{
			return false;
		}
		if ((character.UpgradeTargets ?? new CharacterObject[0]).Any((CharacterObject target) => IsPotentialTroop(target)))
		{
			return true;
		}
		foreach (CharacterObject item in CharacterObject.FindAll(IsPotentialTroop))
		{
			if (item != character && (item.UpgradeTargets ?? new CharacterObject[0]).Contains(character))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsPotentialTroop(CharacterObject character)
	{
		if (character != null && character.IsRegular && character.IsSoldier && !character.IsHero && !character.IsTemplate && !character.IsChildTemplate)
		{
			return !character.HiddenInEncyclopedia;
		}
		return false;
	}

	private static void ExpandTree(CharacterObject selected, TroopRoster rightParty, TroopRoster leftMembers)
	{
		if (selected == null)
		{
			return;
		}
		foreach (CharacterObject item in GetTree(selected))
		{
			if (rightParty.GetTroopCount(item) == 0)
			{
				rightParty.AddToCounts(item, 1);
				if (leftMembers.GetTroopCount(item) > 0)
				{
					leftMembers.AddToCounts(item, -1);
				}
			}
		}
	}

	private static HashSet<CharacterObject> GetTree(CharacterObject anchor)
	{
		HashSet<CharacterObject> hashSet = new HashSet<CharacterObject>();
		Queue<CharacterObject> queue = new Queue<CharacterObject>();
		queue.Enqueue(anchor);
		List<CharacterObject> list = CharacterObject.FindAll(IsRecruitableTroop).ToList();
		while (queue.Count > 0)
		{
			CharacterObject characterObject = queue.Dequeue();
			if (characterObject == null || !hashSet.Add(characterObject))
			{
				continue;
			}
			CharacterObject[] array = characterObject.UpgradeTargets ?? new CharacterObject[0];
			foreach (CharacterObject characterObject2 in array)
			{
				if (IsRecruitableTroop(characterObject2))
				{
					queue.Enqueue(characterObject2);
				}
			}
			foreach (CharacterObject item in list)
			{
				if ((item.UpgradeTargets ?? new CharacterObject[0]).Contains(characterObject))
				{
					queue.Enqueue(item);
				}
			}
		}
		return hashSet;
	}

	private static bool CanTransferTroop(CharacterObject character, PartyScreenLogic.TroopType troopType, PartyScreenLogic.PartyRosterSide side, PartyBase party)
	{
		if (character != null && !character.IsHero)
		{
			return troopType == PartyScreenLogic.TroopType.Member;
		}
		return false;
	}
}
