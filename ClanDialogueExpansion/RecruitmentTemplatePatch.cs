using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;

namespace ClanDialogueExpansion;

[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "ApplyRecruitMercenary")]
internal static class RecruitmentTemplatePatch
{
	private static bool Prefix(MobileParty side1Party, CharacterObject subject, ref int number)
	{
		return RecruitmentRules.CanRecruit(side1Party, subject, ref number);
	}
}
