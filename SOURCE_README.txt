Clan Dialogue Expansion
简体中文源码说明书

工程用途
本工程为《骑马与砍杀 II：霸主》单人模式 Mod「Clan Dialogue Expansion」的发布源码。
功能围绕玩家与家族部队、家族商队的对话扩展，包括长期行动指令、征募模板、部队交换、护航、补给和藏身处清理等。

工程标识
- 工程、程序集、模块 ID、命名空间均为 ClanDialogueExpansion。
- 内部短前缀为 CDE。
- 本发布版不保证与旧版 Mod 的存档数据兼容。

构建环境
- .NET SDK，目标框架 net472，x64。
- 游戏目录：D:\game\Mount.and.Blade.II.Bannerlord-InsaneRamZes。
- 需要游戏本体程序集及 Bannerlord.Harmony。

构建命令
dotnet build ClanDialogueExpansion.csproj -c Release

构建产物
bin\Release\net472\ClanDialogueExpansion.dll
bin\Release\net472\ClanDialogueExpansion.pdb

发布内容
发布 Mod 还需要 SubModule.xml、ModuleData\strings.xml、ModuleData\Languages\CNs\language_data.xml 及 cde_strings.xml。
最终 Mod ZIP 的根目录为 ClanDialogueExpansion，直接解压到游戏 Modules 目录即可。

语言声明
本说明书使用简体中文。ModuleData\strings.xml 是英文游戏文本，ModuleData\Languages\CNs\cde_strings.xml 是简体中文本地化文本。
