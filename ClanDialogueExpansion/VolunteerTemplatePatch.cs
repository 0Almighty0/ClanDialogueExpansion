using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;

namespace ClanDialogueExpansion;

[HarmonyPatch(typeof(RecruitmentCampaignBehavior), "GetRecruitVolunteerFromIndividual")]
internal static class VolunteerTemplatePatch
{
	private static bool Prefix(MobileParty side1Party, CharacterObject subject)
	{
		int number = 1;
		return RecruitmentRules.CanRecruit(side1Party, subject, ref number);
	}
}
