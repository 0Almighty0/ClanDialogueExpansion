using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;

namespace ClanDialogueExpansion;

[HarmonyPatch(typeof(PartyUpgraderCampaignBehavior), "UpgradeTroop")]
internal static class UpgradeTemplatePatch
{
	private static bool Prefix(PartyBase party, object upgradeArgs)
	{
		CharacterObject characterObject = ((upgradeArgs != null) ? Traverse.Create(upgradeArgs).Field("UpgradeTarget").GetValue<CharacterObject>() : null);
		if (characterObject != null)
		{
			return RecruitmentRules.CanUpgrade(party, characterObject);
		}
		return false;
	}
}
