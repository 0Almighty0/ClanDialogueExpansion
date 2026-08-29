using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace ClanDialogueExpansion;

[HarmonyPatch]
internal static class GarrisonDonationPatch
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(AccessTools.TypeByName("DefaultSettlementGarrisonModel"), "FindNumberOfTroopsToLeaveToGarrison", new Type[2]
		{
			typeof(MobileParty),
			typeof(Settlement)
		}, (Type[])null);
	}

	private static void Postfix(MobileParty mobileParty, Settlement settlement, ref int __result)
	{
		PartyOrder partyOrder = CorePartyBehavior.Instance?.GetOrder(mobileParty?.LeaderHero);
		if (partyOrder != null && !partyOrder.AllowDonatingToOtherClanGarrisons && settlement?.OwnerClan != null && mobileParty?.LeaderHero != null && settlement.OwnerClan != mobileParty.LeaderHero.Clan)
		{
			__result = 0;
		}
		if (mobileParty?.Army?.LeaderParty == MobileParty.MainParty && mobileParty != MobileParty.MainParty)
		{
			__result = 0;
		}
	}
}
