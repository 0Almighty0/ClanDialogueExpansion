using System;
using System.Runtime.CompilerServices;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace ClanDialogueExpansion;

internal static class PartyTransferScreen
{
	[Serializable]
	[CompilerGenerated]
	private sealed class LegacyC
	{
		public static readonly LegacyC _003C_003E9 = new LegacyC();

		public static IsTroopTransferableDelegate _003C_003E9__0_0;

		internal bool _003COpen_003Eb__0_0(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase party)
		{
			return true;
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class ModernC
	{
		public static readonly ModernC _003C_003E9 = new ModernC();

		public static IsTroopTransferableDelegate _003C_003E9__1_0;

		internal bool _003COpen_003Eb__1_0(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase party)
		{
			return true;
		}
	}

	public static void Open(MobileParty companionParty)
	{
		if (companionParty == null || MobileParty.MainParty == null)
		{
			return;
		}
		PartyScreenLogic partyScreenLogic = new PartyScreenLogic();
		PartyScreenLogicInitializationData partyScreenLogicInitializationData = new PartyScreenLogicInitializationData
		{
			LeftOwnerParty = companionParty.Party,
			RightOwnerParty = MobileParty.MainParty.Party,
			LeftMemberRoster = companionParty.MemberRoster,
			LeftPrisonerRoster = companionParty.PrisonRoster,
			RightMemberRoster = MobileParty.MainParty.MemberRoster,
			RightPrisonerRoster = MobileParty.MainParty.PrisonRoster,
			LeftLeaderHero = companionParty.LeaderHero,
			RightLeaderHero = Hero.MainHero,
			LeftPartyMembersSizeLimit = companionParty.Party.PartySizeLimit,
			LeftPartyPrisonersSizeLimit = companionParty.Party.PrisonerSizeLimit,
			RightPartyMembersSizeLimit = MobileParty.MainParty.Party.PartySizeLimit,
			RightPartyPrisonersSizeLimit = MobileParty.MainParty.Party.PrisonerSizeLimit,
			LeftPartyName = companionParty.Name,
			RightPartyName = MobileParty.MainParty.Name
		};
		object obj = LegacyC._003C_003E9__0_0;
		if (obj == null)
		{
			object obj2 = (IsTroopTransferableDelegate)((CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase party) => true);
			LegacyC._003C_003E9__0_0 = (IsTroopTransferableDelegate)obj2;
			obj = obj2;
		}
		partyScreenLogicInitializationData.TroopTransferableDelegate = (IsTroopTransferableDelegate)obj;
		partyScreenLogicInitializationData.Header = new TextObject(CdeText.Get("{=cde.core.screen.transfer.header}Manage Troops and Prisoners"));
		partyScreenLogicInitializationData.MemberTransferState = PartyScreenLogic.TransferState.Transferable;
		partyScreenLogicInitializationData.PrisonerTransferState = PartyScreenLogic.TransferState.Transferable;
		partyScreenLogicInitializationData.AccompanyingTransferState = PartyScreenLogic.TransferState.Transferable;
		partyScreenLogicInitializationData.IsTroopUpgradesDisabled = false;
		PartyScreenLogicInitializationData initializationData = partyScreenLogicInitializationData;
		initializationData.PartyScreenMode = PartyScreenHelper.PartyScreenMode.TroopsManage;
		partyScreenLogic.Initialize(initializationData);
		PartyState partyState = Game.Current.GameStateManager.CreateState<PartyState>();
		partyState.PartyScreenLogic = partyScreenLogic;
		partyState.PartyScreenMode = PartyScreenHelper.PartyScreenMode.TroopsManage;
		partyState.IsDonating = false;
		Game.Current.GameStateManager.PushState(partyState);
	}
}
