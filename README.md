# 模组属性翻译优化（ModuleStatLoc）

《深空当铺》(Probably Stolen) 模组。将机器模组属性「性能 / 效率 / 质量」改为更直观的「速度 / 节能 / 品质」，可选在译名后用括号显示原名。

A localization tweak for *Probably Stolen*. Renames module stats 性能 / 效率 / 质量 to 速度 / 节能 / 品质, with an optional original-name suffix.

## 功能

- 性能 → 速度；效率 → 节能；质量 → 品质
- 可选后缀：只显示新译名、新译名（性能）、或新译名（Performance）
- 改完设置后进游戏即生效（无需重启）

设置在游戏「模组」菜单的齿轮里。

## 安装

把编译产物放到游戏的 `Mods/ModuleStatLoc/`（需包含 `manifest.xml`、`ModuleStatLoc.dll` 以及 `Localization/`）。启用后重启游戏。

本仓库是源码，作为作者私有工作区的 Git 子模块编译：项目引用工作区共享的 `build/PsmMod.props`。单独克隆后无法直接 `dotnet build`，请把本目录放到该工作区根目录后再编译，或把已编译的 DLL 与资源复制进游戏 `Mods/`。

## 清单

| 字段 | 值 |
|---|---|
| ID | `module_stat_loc` |
| 游戏版本 | `1.0.0` |
| 能力 | 无 |
