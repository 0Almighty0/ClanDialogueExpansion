using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;

namespace ClanDialogueExpansion;

[HarmonyPatch(typeof(RecruitPrisonersCampaignBehavior), "RecruitPrisonersAi")]
internal static class PrisonerRecruitmentTemplatePatch
{
	private static bool Prefix(MobileParty mobileParty, CharacterObject troop, ref int num)
	{
		if (RecruitmentRules.IsPrisonerRecruitmentBlocked(mobileParty))
		{
			return false;
		}
		return RecruitmentRules.CanRecruit(mobileParty, troop, ref num);
	}
}
