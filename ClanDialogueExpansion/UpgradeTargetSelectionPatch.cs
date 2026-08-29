using System.Collections;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

namespace ClanDialogueExpansion;

[HarmonyPatch(typeof(PartyUpgraderCampaignBehavior), "GetPossibleUpgradeTargets")]
internal static class UpgradeTargetSelectionPatch
{
	private static void Postfix(PartyBase party, TroopRosterElement element, object __result)
	{
		if (!(__result is IList list))
		{
			return;
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			object obj = list[num];
			CharacterObject characterObject = ((obj != null) ? Traverse.Create(obj).Field("UpgradeTarget").GetValue<CharacterObject>() : null);
			if (characterObject == null || !RecruitmentRules.CanUpgrade(party, characterObject))
			{
				list.RemoveAt(num);
			}
		}
	}
}
