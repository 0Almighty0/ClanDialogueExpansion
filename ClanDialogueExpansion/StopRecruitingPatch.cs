using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;

namespace ClanDialogueExpansion;

[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "RecruitVolunteersFromNotable")]
internal static class StopRecruitingPatch
{
	private static bool Prefix(MobileParty mobileParty)
	{
		return !IsRestricted(mobileParty, (PartyOrder x) => x.StopRecruitingTroops);
	}

	private static bool IsRestricted(MobileParty party, Func<PartyOrder, bool> rule)
	{
		PartyOrder partyOrder = CorePartyBehavior.Instance?.GetOrder(party?.LeaderHero);
		if (partyOrder != null)
		{
			return rule(partyOrder);
		}
		return false;
	}
}
