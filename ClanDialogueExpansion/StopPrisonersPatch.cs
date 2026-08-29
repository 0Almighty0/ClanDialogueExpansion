using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

namespace ClanDialogueExpansion;

[HarmonyPatch(typeof(TroopRoster), "AddToCounts", new Type[]
{
	typeof(CharacterObject),
	typeof(int),
	typeof(bool),
	typeof(int),
	typeof(int),
	typeof(bool),
	typeof(int)
})]
internal static class StopPrisonersPatch
{
	private static bool Prefix(TroopRoster __instance, CharacterObject character, int count, ref int __result)
	{
		PartyBase value = Traverse.Create((object)__instance).Property("OwnerParty", (object[])null).GetValue<PartyBase>();
		PartyOrder partyOrder = CorePartyBehavior.Instance?.GetOrder(value?.LeaderHero);
		if (partyOrder != null && partyOrder.StopTakingPrisoners && __instance == value.PrisonRoster && character != null && !character.IsHero && count > 0)
		{
			__result = -1;
			return false;
		}
		return true;
	}
}
