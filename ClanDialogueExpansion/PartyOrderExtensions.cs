using TaleWorlds.CampaignSystem;

namespace ClanDialogueExpansion;

public static class PartyOrderExtensions
{
	public static PartyOrder GetCoreOrder(this Hero hero)
	{
		return CorePartyBehavior.Instance?.GetOrder(hero);
	}

	public static void CancelCoreOrder(this Hero hero)
	{
		CorePartyBehavior.Instance?.CancelOrder(hero);
	}
}
