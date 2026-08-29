using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

namespace ClanDialogueExpansion;

internal static class RecruitmentRules
{
	public static bool IsRecruitmentBlocked(MobileParty party)
	{
		return (CorePartyBehavior.Instance?.GetOrder(party?.LeaderHero))?.StopRecruitingTroops ?? false;
	}

	public static bool CanRecruit(MobileParty party, CharacterObject troop, ref int number)
	{
		if (party == null || troop == null || IsRecruitmentBlocked(party))
		{
			return false;
		}
		TroopRoster troopRoster = CorePartyBehavior.Instance?.GetTemplate(party.LeaderHero);
		if (troopRoster == null)
		{
			return true;
		}
		if (!TryGetTemplateLimit(party.LeaderHero, troopRoster, troop, out var limit))
		{
			return false;
		}
		number = Math.Min(number, limit - party.MemberRoster.GetTroopCount(troop));
		return number > 0;
	}

	public static bool CanUpgrade(PartyBase party, CharacterObject upgradeTarget)
	{
		MobileParty mobileParty = party?.MobileParty;
		if (mobileParty == null || upgradeTarget == null || IsRecruitmentBlocked(mobileParty))
		{
			return false;
		}
		TroopRoster troopRoster = CorePartyBehavior.Instance?.GetTemplate(mobileParty.LeaderHero);
		if (troopRoster == null)
		{
			return true;
		}
		if (!TryGetTemplateLimit(mobileParty.LeaderHero, troopRoster, upgradeTarget, out var limit))
		{
			return false;
		}
		if (mobileParty.MemberRoster.GetTroopCount(upgradeTarget) >= limit)
		{
			return false;
		}
		TroopRoster troopRoster2 = CorePartyBehavior.Instance?.GetTemplateRoots(mobileParty.LeaderHero);
		if (troopRoster2 == null || troopRoster2.TotalManCount == 0)
		{
			return true;
		}
		foreach (TroopRosterElement item in from element in troopRoster2.GetTroopRoster()
			where element.Number > 0
			select element)
		{
			if (CanUpgradeToward(upgradeTarget, item.Character))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsPrisonerRecruitmentBlocked(MobileParty party)
	{
		return (CorePartyBehavior.Instance?.GetOrder(party?.LeaderHero))?.StopTakingPrisoners ?? false;
	}

	private static bool TryGetTemplateLimit(Hero hero, TroopRoster template, CharacterObject troop, out int limit)
	{
		limit = template.GetTroopCount(troop);
		if (limit > 0)
		{
			return true;
		}
		TroopRoster troopRoster = CorePartyBehavior.Instance?.GetTemplateRoots(hero);
		if (troopRoster == null)
		{
			return false;
		}
		foreach (TroopRosterElement item in from element in troopRoster.GetTroopRoster()
			where element.Number > 0
			select element)
		{
			if (GetTree(item.Character).Contains(troop))
			{
				limit = item.Number;
				return limit > 0;
			}
		}
		return false;
	}

	private static bool CanUpgradeToward(CharacterObject current, CharacterObject target)
	{
		if (current == target)
		{
			return true;
		}
		HashSet<CharacterObject> hashSet = new HashSet<CharacterObject>();
		Queue<CharacterObject> queue = new Queue<CharacterObject>();
		queue.Enqueue(current);
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
				if (characterObject2 == target)
				{
					return true;
				}
				queue.Enqueue(characterObject2);
			}
		}
		return false;
	}

	private static HashSet<CharacterObject> GetTree(CharacterObject anchor)
	{
		HashSet<CharacterObject> hashSet = new HashSet<CharacterObject>();
		Queue<CharacterObject> queue = new Queue<CharacterObject>();
		List<CharacterObject> list = CharacterObject.FindAll(IsRecruitableTroop).ToList();
		queue.Enqueue(anchor);
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
}
