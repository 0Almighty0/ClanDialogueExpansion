using System;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;

namespace ClanDialogueExpansion;

internal static class Stage4EscortHelpers
{
	public static bool IsEscortOrder(Hero hero, MobileParty target)
	{
		PartyOrder order = CorePartyBehavior.Instance?.GetOrder(hero);
		return order != null && order.Type == CoreOrderType.FollowPlayer && order.TargetParty == target;
	}

	public static bool IsEngageKeyDown()
	{
		try
		{
			int key = Config.Value.OrderEscortEngageHoldKey;
			return key >= 0 && Input.IsKeyDown((InputKey)key);
		}
		catch (Exception)
		{
			return false;
		}
	}
}

[HarmonyPatch]
internal static class Stage4SetMoveEngagePatch
{
	private static System.Reflection.MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(MobileParty), "SetMoveEngageParty");
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static void Postfix(MobileParty __instance, object[] __args)
	{
		MobileParty party = __args?.OfType<MobileParty>().FirstOrDefault();
		if (__instance == null || party == null || CorePartyBehavior.Instance == null)
		{
			return;
		}
		if (__instance.IsMainParty && Stage4EscortHelpers.IsEngageKeyDown())
		{
			CorePartyBehavior.Instance.CommandEscortAttack(party);
		}
	}
}

[HarmonyPatch(typeof(MobileParty), "SetMoveGoToSettlement", new Type[] { typeof(Settlement), typeof(MobileParty.NavigationType), typeof(bool) })]
internal static class Stage4PlayerSettlementMovePatch
{
	private static void Postfix(MobileParty __instance)
	{
		if (__instance?.IsMainParty == true && Stage4EscortHelpers.IsEngageKeyDown())
		{
			CorePartyBehavior.Instance?.RestoreEscortOrders();
		}
	}
}

[HarmonyPatch]
internal static class Stage4PlayerPointMovePatch
{
	private static System.Reflection.MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(MobileParty), "SetMoveGoToPoint");
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static void Postfix(MobileParty __instance)
	{
		if (__instance?.IsMainParty == true && Stage4EscortHelpers.IsEngageKeyDown())
		{
			CorePartyBehavior.Instance?.RestoreEscortOrders();
		}
	}
}

[HarmonyPatch]
internal static class Stage4BattleEndedPatch
{
	private static System.Reflection.MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(MobileParty), "OnEventEnded");
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static void Postfix(MobileParty __instance)
	{
		PartyOrder order = CorePartyBehavior.Instance?.GetOrder(__instance?.LeaderHero);
		if (order?.Type == CoreOrderType.FollowPlayer)
		{
			CorePartyBehavior.Instance.RestoreOrderMovement(__instance);
		}
	}
}
