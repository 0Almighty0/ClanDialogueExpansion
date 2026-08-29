using System;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace ClanDialogueExpansion;

internal static class PartyEquipmentScreen
{
	public static void Open(MobileParty companionParty)
	{
		if (companionParty == null || MobileParty.MainParty == null || companionParty.LeaderHero == null)
		{
			return;
		}
		try
		{
			InventoryScreenHelper.OpenScreenAsInventoryOf(MobileParty.MainParty.Party, companionParty.Party, companionParty.LeaderHero.CharacterObject, companionParty.Name);
		}
		catch (Exception ex)
		{
			InformationManager.DisplayMessage(new InformationMessage(CdeText.Get("{=cde.core.error.equipment}Party equipment screen could not be opened:") + " " + ex.Message));
		}
	}
}
