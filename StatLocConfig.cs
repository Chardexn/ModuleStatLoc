namespace ModuleStatLoc
{
    internal enum SuffixMode
    {
        None = 0,
        Original = 1,
        English = 2
    }

    /// <summary>
    /// 模组配置。Define 在 Init 调用；标签用延迟 Func，打开面板时才读本地化。
    /// </summary>
    internal static class StatLocConfig
    {
        public static ConfigEntry<SuffixMode> SuffixEntry { get; private set; }

        public static SuffixMode Suffix => SuffixEntry != null ? SuffixEntry.Value : SuffixMode.None;

        #region MARK: 定义
        public static void Define()
        {
            ModConfig cfg = ModHelper.GetConfig(ModEntry.MOD_ID);
            SuffixEntry = cfg.DefineEnum(
                "name_suffix",
                SuffixMode.None,
                "General",
                () => ModEntry.L("cfg_suffix"),
                () => ModEntry.L("cfg_suffix_tip"));
        }
        #endregion
    }
}
