using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;

namespace ClanDialogueExpansion;

public sealed class PartyOrder
{
	[SaveableProperty(1)]
	public Hero Owner { get; private set; }

	[SaveableProperty(2)]
	public CoreOrderType Type { get; private set; }

	[SaveableProperty(3)]
	public MobileParty TargetParty { get; private set; }

	[SaveableProperty(4)]
	public Settlement TargetSettlement { get; private set; }

	[SaveableProperty(5)]
	public Settlement ResupplySettlement { get; private set; }

	[SaveableProperty(6)]
	public bool StopRecruitingTroops { get; set; }

	[SaveableProperty(7)]
	public bool StopTakingPrisoners { get; set; }

	[SaveableProperty(8)]
	public bool AllowRaidingVillages { get; set; } = true;

	[SaveableProperty(9)]
	public bool AllowSieges { get; set; } = true;

	[SaveableProperty(10)]
	public bool AllowJoiningArmies { get; set; } = true;

	[SaveableProperty(11)]
	public bool AllowDonatingToOtherClanGarrisons { get; set; } = true;

	[SaveableProperty(12)]
	public bool AllowClearingHideouts { get; set; } = true;

	public PartyOrder(Hero owner, CoreOrderType type)
	{
		Owner = owner;
		Type = type;
	}

	public void BeginResupply(Settlement settlement)
	{
		ResupplySettlement = settlement;
	}

	public void EndResupply()
	{
		ResupplySettlement = null;
	}

	public void SetTargetSettlement(Settlement settlement)
	{
		TargetSettlement = settlement;
	}

	public static PartyOrder Follow(Hero owner, MobileParty target)
	{
		return new PartyOrder(owner, CoreOrderType.FollowPlayer)
		{
			TargetParty = target
		};
	}

	public static PartyOrder Patrol(Hero owner, Settlement target)
	{
		return new PartyOrder(owner, CoreOrderType.PatrolSettlement)
		{
			TargetSettlement = target
		};
	}

	public static PartyOrder Stay(Hero owner, Settlement target)
	{
		return new PartyOrder(owner, CoreOrderType.StayInSettlement)
		{
			TargetSettlement = target
		};
	}

	public static PartyOrder RulesOnly(Hero owner)
	{
		return new PartyOrder(owner, CoreOrderType.RulesOnly);
	}

	public static PartyOrder Roam(Hero owner)
	{
		return new PartyOrder(owner, CoreOrderType.Roam);
	}

	public static PartyOrder ClearNearbyHideout(Hero owner, Settlement target)
	{
		return new PartyOrder(owner, CoreOrderType.ClearNearbyHideout)
		{
			TargetSettlement = target
		};
	}
}
