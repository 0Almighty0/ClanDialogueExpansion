using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace ClanDialogueExpansion;

public sealed class SubModule : MBSubModuleBase
{
	protected override void OnSubModuleLoad()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		base.OnSubModuleLoad();
		new Harmony("ClanDialogueExpansion").PatchAll(typeof(SubModule).Assembly);
	}

	protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
	{
		base.OnGameStart(game, gameStarterObject);
		if (game.GameType is Campaign)
		{
			((CampaignGameStarter)((gameStarterObject is CampaignGameStarter) ? gameStarterObject : null))?.AddBehavior(new CorePartyBehavior());
		}
	}
}
