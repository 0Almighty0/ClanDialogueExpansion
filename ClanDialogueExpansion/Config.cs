using System;

namespace ClanDialogueExpansion;

[Serializable]
public sealed class Config
{
	public static Config Value => ConfigLoader.Instance.Config;

	public float RelationRaidingPositiveMultMin { get; set; } = 0.3f;
	public float RelationRaidingNegativeMultMax { get; set; } = 1.5f;
	public float RelationSiegingPositiveMultMin { get; set; } = 0.5f;
	public float RelationSiegingNegativeMultMax { get; set; } = 1.5f;
	public float SameCultureRaidingMult { get; set; } = 0.5f;
	public float SameCultureSiegingMult { get; set; } = 1.5f;
	public float PartyMaintenanceScoreMultiplier { get; set; } = 1f;
	public float OwnClanVillagesScoreMultiplier { get; set; } = 1f;
	public float FriendlyVillagesScoreMultiplier { get; set; } = 1f;
	public float HostileSettlementsScoreMultiplier { get; set; } = 1f;
	public float AttackInitiativeMultiplier { get; set; } = 1f;
	public float AvoidInitiativeMultiplier { get; set; } = 1f;
	public float CommandScoreMinimum { get; set; } = 1000f;
	public float CommandScoreMultiplier { get; set; } = 1f;
	public float ShortTermReactionRangeMultiplier { get; set; } = 1f;
	public float BorderTargetDistanceMultiplier { get; set; } = 1f;
	public bool EnableBorderOnlySieges { get; set; } = true;
	public int OrderEscortEngageHoldKey { get; set; } = 56;
	public float MinimumDaysFoodToLastWhileBuyingFood { get; set; } = 15f;
	public float ResupplyTriggerDays { get; set; } = 2f;
	public float ResupplyTargetDays { get; set; } = 7f;
	public int SpareMountsToKeep { get; set; } = 5;
	public int ClanPartyGoldLimitToTakeFromTreasury { get; set; } = 200;
}
