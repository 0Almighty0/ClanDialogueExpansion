using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace ClanDialogueExpansion;

// Stage 3 changes only scores produced by the native AI. Orders still use the
// movement methods in CorePartyBehavior, so this patch does not take ownership
// of the party navigation state.
[HarmonyPatch]
internal static class Stage3AiMilitaryScorePatch
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(AiMilitaryBehavior), "AiHourlyTick", new Type[2]
		{
			typeof(MobileParty),
			typeof(PartyThinkParams)
		});
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static void Postfix(MobileParty mobileParty, PartyThinkParams p)
	{
		Hero leader = mobileParty?.LeaderHero;
		PartyOrder order = CorePartyBehavior.Instance?.GetOrder(leader);
		if (leader == null || order == null || p?.AIBehaviorScores == null)
		{
			return;
		}
		bool commandTargetFound = false;
		for (int i = 0; i < p.AIBehaviorScores.Count; i++)
		{
			(AIBehaviorData data, float score) = p.AIBehaviorScores[i];
			if (data.Party == null)
			{
				continue;
			}

			Settlement settlement = data.Party as Settlement;
			if (order.Type == CoreOrderType.RulesOnly && data.AiBehavior == AiBehavior.EscortParty && data.Party == MobileParty.MainParty)
			{
				p.SetBehaviorScore(data, 0f);
				continue;
			}
			switch (data.AiBehavior)
			{
				case AiBehavior.GoToSettlement:
					score *= Config.Value.PartyMaintenanceScoreMultiplier;
					if (settlement != null && !order.AllowDonatingToOtherClanGarrisons)
					{
						// Keep towns useful for food and trade, but deprioritize an
						// under-strength garrison that this party cannot reinforce.
						score *= GetGarrisonMaintenanceMultiplier(mobileParty, settlement, leader);
					}
					break;

				case AiBehavior.DefendSettlement:
				case AiBehavior.PatrolAroundPoint:
					if (settlement != null)
					{
						score *= settlement.OwnerClan == leader.Clan
							? Config.Value.OwnClanVillagesScoreMultiplier
							: Config.Value.FriendlyVillagesScoreMultiplier;
					}
					break;

				case AiBehavior.BesiegeSettlement:
				case AiBehavior.AssaultSettlement:
					score *= Config.Value.HostileSettlementsScoreMultiplier;
					if (!order.AllowSieges)
					{
						score = 0f;
					}
					break;

				case AiBehavior.RaidSettlement:
					if (!order.AllowRaidingVillages)
					{
						score = 0f;
					}
					break;
			}

			if (IsCommandTarget(order, data))
			{
				score = ApplyCommandScore(score);
				commandTargetFound = true;
			}

			p.SetBehaviorScore(data, score);
		}

		// Native AI normally supplies these entries, but it can omit a target
		// while a party is transitioning between behaviors. Re-add the command
		// target so the order remains enforceable without forcing a movement API.
		if (!commandTargetFound)
		{
			TryAddCommandTarget(p, order);
		}
	}

	private static bool IsCommandTarget(PartyOrder order, AIBehaviorData data)
	{
		if (order.Type == CoreOrderType.FollowPlayer)
		{
			return data.AiBehavior == AiBehavior.EscortParty && data.Party == order.TargetParty;
		}
		if (order.Type == CoreOrderType.PatrolSettlement)
		{
			return data.AiBehavior == AiBehavior.PatrolAroundPoint && data.Party == order.TargetSettlement;
		}
		return order.Type == CoreOrderType.StayInSettlement && data.AiBehavior == AiBehavior.GoToSettlement && data.Party == order.TargetSettlement;
	}

	private static float ApplyCommandScore(float score)
	{
		float multiplier = Config.Value.CommandScoreMultiplier;
		float minimum = Config.Value.CommandScoreMinimum;
		if (float.IsNaN(multiplier) || float.IsInfinity(multiplier) || multiplier < 0f)
		{
			multiplier = 1f;
		}
		if (float.IsNaN(minimum) || float.IsInfinity(minimum) || minimum < 0f)
		{
			minimum = 0f;
		}
		return Math.Max(score * multiplier, minimum);
	}

	private static void TryAddCommandTarget(PartyThinkParams p, PartyOrder order)
	{
		if (order.Type == CoreOrderType.FollowPlayer && order.TargetParty != null && order.TargetParty.IsActive)
		{
			AddCommandTarget(p, new AIBehaviorData(order.TargetParty, AiBehavior.EscortParty, MobileParty.NavigationType.Default, false, false, false));
		}
		else if (order.Type == CoreOrderType.PatrolSettlement && order.TargetSettlement != null && order.TargetSettlement.IsActive)
		{
			AddCommandTarget(p, new AIBehaviorData(order.TargetSettlement, AiBehavior.PatrolAroundPoint, MobileParty.NavigationType.Default, false, false, false));
		}
		else if (order.Type == CoreOrderType.StayInSettlement && order.TargetSettlement != null && order.TargetSettlement.IsActive)
		{
			AddCommandTarget(p, new AIBehaviorData(order.TargetSettlement, AiBehavior.GoToSettlement, MobileParty.NavigationType.Default, false, false, false));
		}
	}

	private static void AddCommandTarget(PartyThinkParams p, AIBehaviorData data)
	{
		(AIBehaviorData, float) value = (data, ApplyCommandScore(0f));
		p.AddBehaviorScore(in value);
	}

	private static float GetGarrisonMaintenanceMultiplier(MobileParty mobileParty, Settlement settlement, Hero leader)
	{
		if (mobileParty == null || settlement == null || leader?.Clan == null || settlement.IsVillage || settlement.OwnerClan == null || settlement.OwnerClan.Kingdom != leader.Clan.Kingdom)
		{
			return 1f;
		}
		if (!settlement.IsFortification || settlement.OwnerClan == Clan.PlayerClan || settlement.Town == null)
		{
			return 1f;
		}

		Kingdom kingdom = leader.Clan.Kingdom;
		if (kingdom == null)
		{
			return 1f;
		}
		float idealStrength = FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(kingdom, null);
		if (mobileParty.Army != null)
		{
			idealStrength *= 0.75f;
		}
		float currentStrength = settlement.Town.GarrisonParty?.Party?.EstimatedStrength ?? 0f;
		float targetStrength = idealStrength
			* FactionHelper.OwnerClanEconomyEffectOnGarrisonSizeConstant(settlement.OwnerClan)
			* FactionHelper.SettlementProsperityEffectOnGarrisonSizeConstant(settlement.Town)
			* FactionHelper.SettlementFoodPotentialEffectOnGarrisonSizeConstant(settlement);
		if (targetStrength <= 0f || currentStrength >= targetStrength)
		{
			return 1f;
		}
		float shortage = Math.Max(0f, 1f - currentStrength / targetStrength);
		return 1f / (1f + (float)Math.Pow(shortage, 3d) * 99f);
	}
}

[HarmonyPatch]
internal static class Stage3TargetScorePatch
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(DefaultTargetScoreCalculatingModel), "GetTargetScoreForFaction", new Type[4]
		{
			typeof(Settlement),
			typeof(Army.ArmyTypes),
			typeof(MobileParty),
			typeof(float)
		});
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static void Postfix(Settlement targetSettlement, Army.ArmyTypes missionType, MobileParty mobileParty, ref float __result)
	{
		Hero leader = mobileParty?.LeaderHero;
		if (targetSettlement == null || leader == null || __result <= 0f || (missionType != Army.ArmyTypes.Raider && missionType != Army.ArmyTypes.Besieger))
		{
			return;
		}

		Hero owner = targetSettlement.OwnerClan?.Leader;
		if (owner != null)
		{
			float relation = Math.Max(-1f, Math.Min(1f, owner.GetRelation(leader) / 20f));
			float positiveMinimum = missionType == Army.ArmyTypes.Raider
				? Config.Value.RelationRaidingPositiveMultMin
				: Config.Value.RelationSiegingPositiveMultMin;
			float negativeMaximum = missionType == Army.ArmyTypes.Raider
				? Config.Value.RelationRaidingNegativeMultMax
				: Config.Value.RelationSiegingNegativeMultMax;
			float relationMultiplier = 1f - relation * (relation > 0f ? 1f - positiveMinimum : negativeMaximum - 1f);
			__result *= Math.Max(0f, relationMultiplier);
		}

		if (missionType == Army.ArmyTypes.Raider && (targetSettlement.Culture == leader.Culture || targetSettlement.Culture == mobileParty.MapFaction?.Culture))
		{
			__result *= Config.Value.SameCultureRaidingMult;
		}
		else if (missionType == Army.ArmyTypes.Besieger && targetSettlement.Culture == mobileParty.MapFaction?.Culture)
		{
			__result *= Config.Value.SameCultureSiegingMult;
		}
	}
}

[HarmonyPatch]
internal static class Stage3SiegeTargetPriorityPatch
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(AiMilitaryBehavior), "FindBestTargetAndItsValueForFaction", new Type[3]
		{
			typeof(Army.ArmyTypes),
			typeof(PartyThinkParams),
			typeof(float)
		});
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static void Postfix(Army.ArmyTypes missionType, PartyThinkParams p)
	{
		if (!Config.Value.EnableBorderOnlySieges || missionType != Army.ArmyTypes.Besieger || p?.MobilePartyOf == null)
		{
			return;
		}

		MobileParty party = p.MobilePartyOf;
		if (party.LeaderHero == null)
		{
			return;
		}

		Settlement home = party.LeaderHero.HomeSettlement ?? party.LastVisitedSettlement;
		home = home ?? party.MapFaction?.FactionMidSettlement;
		if (home == null)
		{
			return;
		}

		List<(AIBehaviorData Data, float Score, Settlement Target, float Distance, bool IsBorder)> targets = new List<(AIBehaviorData, float, Settlement, float, bool)>();
		float closestDistance = float.MaxValue;
		Settlement closestTarget = null;
		foreach ((AIBehaviorData data, float score) in p.AIBehaviorScores)
		{
			Settlement target = data.Party as Settlement;
			if (target == null || data.AiBehavior != AiBehavior.BesiegeSettlement || score <= 0f)
			{
				continue;
			}

			float distance = home.GetPosition2D.Distance(target.GetPosition2D);
			targets.Add((data, score, target, distance, IsBorderTarget(party, target)));
			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestTarget = target;
			}
		}

		if (closestTarget == null)
		{
			return;
		}

		bool hasBorderTarget = targets.Any((ValueTuple<AIBehaviorData, float, Settlement, float, bool> x) => x.Item5);
		if (hasBorderTarget)
		{
			foreach ((AIBehaviorData data, float score, Settlement target, float distance, bool isBorder) in targets)
			{
				if (!isBorder)
				{
					p.SetBehaviorScore(data, 0f);
				}
			}
			targets = targets.Where((ValueTuple<AIBehaviorData, float, Settlement, float, bool> x) => x.Item5).ToList();
			closestDistance = targets.Min((ValueTuple<AIBehaviorData, float, Settlement, float, bool> x) => x.Item4);
			closestTarget = targets.OrderBy((ValueTuple<AIBehaviorData, float, Settlement, float, bool> x) => x.Item4).First().Item3;
		}

		float distanceScale = 33.3333f * Math.Max(0.1f, Config.Value.BorderTargetDistanceMultiplier);
		if (distanceScale <= 0f || float.IsNaN(distanceScale) || float.IsInfinity(distanceScale))
		{
			distanceScale = 1f;
		}
		foreach ((AIBehaviorData data, float score, Settlement target, float distance, bool isBorder) in targets)
		{
			float scale = target.Culture == party.MapFaction?.Culture && closestTarget.Culture != party.MapFaction?.Culture
				? distanceScale
				: distanceScale / 3f;
			float distanceFactor = Math.Max(0f, 1f - (distance - closestDistance) / Math.Max(0.1f, scale));
			p.SetBehaviorScore(data, score * 1.2f * distanceFactor);
		}
	}

	private static bool IsBorderTarget(MobileParty party, Settlement target)
	{
		if (party?.MapFaction == null || target?.MapFaction == null || !target.IsFortification)
		{
			return false;
		}
		if (!FactionManager.IsAtWarAgainstFaction(party.MapFaction, target.MapFaction))
		{
			return false;
		}

		float borderDistance = 33.3333f * Math.Max(0.1f, Config.Value.BorderTargetDistanceMultiplier);
		if (borderDistance <= 0f || float.IsNaN(borderDistance) || float.IsInfinity(borderDistance))
		{
			borderDistance = 1f;
		}
		foreach (Settlement friendly in Campaign.Current.Settlements)
		{
			if (friendly == null || !friendly.IsActive || !friendly.IsFortification || friendly.MapFaction != party.MapFaction)
			{
				continue;
			}
			if (friendly.GetPosition2D.Distance(target.GetPosition2D) <= borderDistance)
			{
				return true;
			}
		}
		return false;
	}
}

[HarmonyPatch]
internal static class Stage3ArmyMembershipPatch
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(AiArmyMemberBehavior), "AiHourlyTick", new Type[2]
		{
			typeof(MobileParty),
			typeof(PartyThinkParams)
		});
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static bool Prefix(MobileParty mobileParty)
	{
		if (mobileParty?.Army == null || mobileParty.Army.LeaderParty == mobileParty)
		{
			return true;
		}

		PartyOrder order = CorePartyBehavior.Instance?.GetOrder(mobileParty.LeaderHero);
		Army army = mobileParty.Army;
		if (order != null && army != null && army.LeaderParty != mobileParty)
		{
			bool isPlayerArmy = army.LeaderParty == MobileParty.MainParty;
			// Opening the long-term rules menu creates a rules-only order. That
			// order must preserve membership in the player's current army.
			if (!order.AllowJoiningArmies && !isPlayerArmy)
			{
				ReleaseFromArmy(mobileParty);
				return false;
			}
		}
		return true;
	}

	private static void ReleaseFromArmy(MobileParty mobileParty)
	{
		if (mobileParty == null || mobileParty.Army == null)
		{
			return;
		}
		mobileParty.Army = null;
		mobileParty.AttachedTo = null;
		mobileParty.SetMoveModeHold();
		mobileParty.Ai.SetDoNotMakeNewDecisions(false);
		mobileParty.Ai.RethinkAtNextHourlyTick = true;
	}
}

[HarmonyPatch]
internal static class Stage3ShortTermReactionPatch
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(DefaultMobilePartyAIModel), "GetBestInitiativeBehavior", new Type[5]
		{
			typeof(MobileParty),
			typeof(AiBehavior).MakeByRefType(),
			typeof(MobileParty).MakeByRefType(),
			typeof(float).MakeByRefType(),
			typeof(Vec2).MakeByRefType()
		});
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static void Postfix(MobileParty mobileParty, ref AiBehavior bestInitiativeBehavior, ref MobileParty bestInitiativeTargetParty, ref float bestInitiativeBehaviorScore)
	{
		PartyOrder order = CorePartyBehavior.Instance?.GetOrder(mobileParty?.LeaderHero);
		if (mobileParty == null || order == null)
		{
			return;
		}

		TrySelectExtendedTarget(mobileParty, ref bestInitiativeBehavior, ref bestInitiativeTargetParty, ref bestInitiativeBehaviorScore);
		if (bestInitiativeTargetParty == null || bestInitiativeBehavior != AiBehavior.EngageParty || !bestInitiativeTargetParty.IsBandit)
		{
			return;
		}

		if (order.Type == CoreOrderType.PatrolSettlement || (order.Type == CoreOrderType.FollowPlayer && order.TargetParty != null))
		{
			Vec2 relativeTargetPosition = bestInitiativeTargetParty.VisualPosition2DWithoutError - mobileParty.GetPosition2D;
			bool targetIsMovingTowardParty = bestInitiativeTargetParty.Bearing.DotProduct(relativeTargetPosition) <= 0f;
			if (mobileParty.Speed > bestInitiativeTargetParty.Speed * 1.05f || targetIsMovingTowardParty)
			{
				bestInitiativeBehaviorScore = Math.Max(bestInitiativeBehaviorScore, 1.1f);
			}
		}
	}

	private static void TrySelectExtendedTarget(MobileParty mobileParty, ref AiBehavior bestInitiativeBehavior, ref MobileParty bestInitiativeTargetParty, ref float bestInitiativeBehaviorScore)
	{
		float rangeMultiplier = Config.Value.ShortTermReactionRangeMultiplier;
		if (float.IsNaN(rangeMultiplier) || float.IsInfinity(rangeMultiplier) || rangeMultiplier < 1f)
		{
			rangeMultiplier = 1f;
		}
		float range = Math.Max(6f, mobileParty.SeeingRange * rangeMultiplier);
		float nativeRange = 6f;
		float bestScore = bestInitiativeBehavior == AiBehavior.EngageParty ? bestInitiativeBehaviorScore : 0f;
		MobileParty bestTarget = bestInitiativeBehavior == AiBehavior.EngageParty ? bestInitiativeTargetParty : null;

		foreach (MobileParty candidate in Campaign.Current.MobileParties)
		{
			if (!IsExtendedTarget(mobileParty, candidate))
			{
				continue;
			}
			float distance = mobileParty.GetPosition2D.Distance(candidate.GetPosition2D);
			if (distance <= nativeRange || distance > range)
			{
				continue;
			}
			if (!TryGetInitiativeScore(mobileParty, candidate, out float attackScore) || attackScore <= bestScore)
			{
				continue;
			}
			bestScore = attackScore;
			bestTarget = candidate;
		}

		if (bestTarget != null && bestScore > bestInitiativeBehaviorScore)
		{
			bestInitiativeBehavior = AiBehavior.EngageParty;
			bestInitiativeTargetParty = bestTarget;
			bestInitiativeBehaviorScore = bestScore;
		}
	}

	private static bool IsExtendedTarget(MobileParty source, MobileParty target)
	{
		if (target == null || target == source || !target.IsActive || target.LeaderHero == null || target.IsVillager || target.IsCaravan || target.IsMilitia)
		{
			return false;
		}
		if (target.IsBandit)
		{
			return true;
		}
		return source.MapFaction != null && target.MapFaction != null && FactionManager.IsAtWarAgainstFaction(source.MapFaction, target.MapFaction);
	}

	private static bool TryGetInitiativeScore(MobileParty source, MobileParty target, out float attackScore)
	{
		attackScore = 0f;
		try
		{
			DefaultMobilePartyAIModel model = Campaign.Current.Models.MobilePartyAIModel as DefaultMobilePartyAIModel;
			MethodInfo method = model == null
				? null
				: AccessTools.Method(model.GetType(), "CalculateInitiativeScoresForEnemy", new Type[6]
				{
					typeof(MobileParty),
					typeof(MobileParty),
					typeof(float).MakeByRefType(),
					typeof(float).MakeByRefType(),
					typeof(float),
					typeof(float)
				});
			if (method == null)
			{
				return false;
			}
			object[] args = new object[6] { source, target, 0f, 0f, 0f, 1f };
			method.Invoke(model, args);
			attackScore = Convert.ToSingle(args[3]);
			return !float.IsNaN(attackScore) && !float.IsInfinity(attackScore);
		}
		catch (Exception)
		{
			return false;
		}
	}
}

[HarmonyPatch]
internal static class Stage3InitiativeScorePatch
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(DefaultMobilePartyAIModel), "CalculateInitiativeScoresForEnemy", new Type[6]
		{
			typeof(MobileParty),
			typeof(MobileParty),
			typeof(float).MakeByRefType(),
			typeof(float).MakeByRefType(),
			typeof(float),
			typeof(float)
		});
	}

	private static bool Prepare()
	{
		return TargetMethod() != null;
	}

	private static void Postfix(MobileParty mobileParty, MobileParty enemyParty, ref float avoidScore, ref float attackScore)
	{
		PartyOrder order = CorePartyBehavior.Instance?.GetOrder(mobileParty?.LeaderHero);
		if (order == null || enemyParty == null || mobileParty == enemyParty)
		{
			return;
		}

		float attackMultiplier = Config.Value.AttackInitiativeMultiplier;
		float avoidMultiplier = Config.Value.AvoidInitiativeMultiplier;
		if (float.IsNaN(attackMultiplier) || float.IsInfinity(attackMultiplier) || attackMultiplier < 0f)
		{
			attackMultiplier = 1f;
		}
		if (float.IsNaN(avoidMultiplier) || float.IsInfinity(avoidMultiplier) || avoidMultiplier < 0f)
		{
			avoidMultiplier = 1f;
		}
		attackScore *= attackMultiplier;
		avoidScore *= avoidMultiplier;
	}
}
