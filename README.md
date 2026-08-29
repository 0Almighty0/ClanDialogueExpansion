# Clan Dialogue Expansion

Clan Dialogue Expansion is a single-player mod for Mount & Blade II: Bannerlord 1.4.8. It expands dialogue and management options for clan parties and clan caravans.

The project is inspired by the earlier **Party AI Overhaul and Commands** mod. It was reconstructed from available DLL/PDB information and testing, then reorganized under a new module identity.

## Features

- Issue follow, patrol, settlement stay, roam, army, disband, and order-cancellation commands to clan parties.
- Configure standing rules for recruiting, prisoners, raids, sieges, armies, garrison donations, and hideout clearing.
- Create and edit troop-count recruitment templates using the player, a party, or another clan hero's party.
- Exchange troops, prisoners, equipment, and goods with clan parties.
- Manage guards in caravans owned by the player's clan.
- Support patrol hideout clearing, escort behavior, movement, supply priorities, and excess-horse handling.

## Requirements

- Mount & Blade II: Bannerlord 1.4.8
- Bannerlord.Harmony, loaded before this mod

## Languages

The mod contains English and Simplified Chinese localization. Bannerlord automatically selects the appropriate text according to the game language.

## Build

```powershell
dotnet build ClanDialogueExpansion.csproj -c Release
```

The resulting DLL and PDB are placed in `bin\Release\net472`.

## Releases

Download the Mod ZIP from the repository Releases page. It contains the ready-to-install module and a Simplified Chinese mod manual.

## Compatibility and Maintenance

This release uses the `ClanDialogueExpansion` module ID and `CDE_` save-data keys. It does not guarantee compatibility with standing orders or recruitment templates from earlier versions.

This source code is provided for reference and self-maintenance. Future updates, fixes, and compatibility support are not guaranteed.

---

# 家族对话扩展

这是《骑马与砍杀 II：霸主》1.4.8 的单人模式 Mod，用于扩展玩家与家族部队、家族商队之间的对话和管理功能。

本项目参考早期 Mod **Party AI Overhaul and Commands** 的功能设计与部分逻辑，通过可用 DLL、PDB 信息和测试重建源码，并使用独立的新模块标识重新整理。

## 前置

- 《骑马与砍杀 II：霸主》1.4.8
- `Bannerlord.Harmony`，并在启动器中排在本 Mod 前加载

## 语言

Mod 内置英文与简体中文文本，游戏会根据当前语言设置自动切换，无需安装不同语言版本。

## 说明

请从 Releases 页面下载 Mod 压缩包。源码仅供参考与自行维护；后续不保证持续维护、兼容性更新或错误修复。
