# 模组属性翻译优化

《深空当铺》模组：把机器模组上的「性能 / 效率 / 质量」改成更好懂的「速度 / 节能 / 品质」。可选在后面用括号标出原名。

A *Probably Stolen* localization tweak for machine-module stat names.

**适配游戏：** 1.0.0.13（商店标签 100-Rev13）  
**作者：** kirkxia

## 做什么

原版三个词容易混：

| 原名 | 本模组 |
|---|---|
| 性能 | 速度 |
| 效率 | 节能 |
| 质量 | 品质 |

只改你看见的字，不改数值、不改机器行为。

## 怎么用

1. 把模组文件夹放到游戏的 `Mods\` 下，启动游戏，在「模组」菜单里**启用**，然后**重启**一次。
2. 之后看模组、机器说明时就是新译名。
3. 想对照原文时，打开模组齿轮，把「译名后显示原名」改成「原名」或「英文」。改完进存档就会生效。

## 设置

游戏主菜单 → 模组 → 本模组右侧齿轮：

| 选项 | 效果 |
|---|---|
| 不显示 | 只显示「速度 / 节能 / 品质」 |
| 原名 | 速度（性能） |
| 英文 | 速度（Performance） |

## 安装

把整个模组文件夹放到：

`…\steamapps\common\Probably Stolen\Mods\ModuleStatLoc\`

文件夹里至少要有 `manifest.xml`、`ModuleStatLoc.dll`、`Localization\`。启用后必须重启游戏。

从源码编译（需要 [.NET SDK](https://dotnet.microsoft.com/download) 6.0+ 和一份正式版游戏）：

```powershell
dotnet build -c Release
```

会生成 `dist\ModuleStatLoc\`，并自动复制到游戏的 `Mods\ModuleStatLoc\`。游戏不在默认 Steam 路径时，复制 `GameDir.local.props.example` 为 `GameDir.local.props` 并改里面的路径。

## 卸载

在模组菜单里停用，重启即可。只改显示用的文本，不影响存档。
