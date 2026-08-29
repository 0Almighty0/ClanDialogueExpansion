using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace ClanDialogueExpansion;

// The companion tracker is an internal campaign behavior and its concrete
// type can move between Bannerlord revisions.  Resolve it dynamically so a
// missing method disables only this compatibility patch.
[HarmonyPatch]
internal static class Stage5ScatteredCompanionPatch
{
	private static MethodBase TargetMethod()
	{
		Type type = AccessTools.TypeByName("TaleWorlds.CampaignSystem.CampaignBehaviors.PlayerTrackCompanionBehavior");
		return type == null ? null : AccessTools.Method(type, "AddHeroToScatteredCompanions", new[] { typeof(Hero) });
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static bool Prefix(Hero hero)
	{
		return hero == null || CorePartyBehavior.Instance?.GetOrder(hero) == null;
	}
}

[HarmonyPatch]
internal static class Stage5RemoveCompanionPatch
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(RemoveCompanionAction), "ApplyInternal");
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static void Postfix(object[] __args)
	{
		try
		{
			Clan clan = __args?.OfType<Clan>().FirstOrDefault();
			Hero hero = __args?.OfType<Hero>().FirstOrDefault();
			if (clan == Clan.PlayerClan)
			{
				CorePartyBehavior.Instance?.ClearCompanionData(hero);
			}
		}
		catch
		{
			// Companion removal must never turn into a campaign-load crash.
		}
	}
}
