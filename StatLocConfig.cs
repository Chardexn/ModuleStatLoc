namespace ModuleStatLoc
{
    internal enum SuffixMode
    {
        None = 0,
        Original = 1,
        English = 2
    }

    internal enum PerformanceTerm
    {
        Speed = 0,
        Vanilla = 1,
        Red = 2,
        Yield = 3
    }

    internal enum EfficiencyTerm
    {
        EnergySave = 0,
        Vanilla = 1,
        Green = 2,
        EnergyEff = 3
    }

    internal enum QualityTerm
    {
        Grade = 0,
        Vanilla = 1,
        Yellow = 2,
        Precision = 3
    }

    /// <summary>
    /// 模组配置。Define 在 Init 调用；标签用延迟 Func，打开面板时才读本地化。
    /// </summary>
    internal static class StatLocConfig
    {
        public static ConfigEntry<SuffixMode> SuffixEntry { get; private set; }
        public static ConfigEntry<PerformanceTerm> PerformanceEntry { get; private set; }
        public static ConfigEntry<EfficiencyTerm> EfficiencyEntry { get; private set; }
        public static ConfigEntry<QualityTerm> QualityEntry { get; private set; }

        public static SuffixMode Suffix => SuffixEntry != null ? SuffixEntry.Value : SuffixMode.None;

        public static PerformanceTerm Performance =>
            PerformanceEntry != null ? PerformanceEntry.Value : PerformanceTerm.Speed;

        public static EfficiencyTerm Efficiency =>
            EfficiencyEntry != null ? EfficiencyEntry.Value : EfficiencyTerm.EnergySave;

        public static QualityTerm Quality =>
            QualityEntry != null ? QualityEntry.Value : QualityTerm.Grade;

        public static readonly string[] PerformanceKeys =
        {
            "opt_perf_speed",
            "opt_perf_vanilla",
            "opt_perf_red",
            "opt_perf_yield"
        };

        public static readonly string[] EfficiencyKeys =
        {
            "opt_eff_energy",
            "opt_eff_vanilla",
            "opt_eff_green",
            "opt_eff_energy_eff"
        };

        public static readonly string[] QualityKeys =
        {
            "opt_qual_grade",
            "opt_qual_vanilla",
            "opt_qual_yellow",
            "opt_qual_precision"
        };

        public static readonly string[] SuffixKeys =
        {
            "opt_suffix_none",
            "opt_suffix_original",
            "opt_suffix_english"
        };

        #region MARK: 定义
        public static void Define()
        {
            ModConfig cfg = ModHelper.GetConfig(ModEntry.MOD_ID);
            const string section = "General";

            PerformanceEntry = cfg.DefineEnum(
                "term_performance",
                PerformanceTerm.Speed,
                section,
                () => ModEntry.L("cfg_perf"),
                () => ModEntry.L("cfg_perf_tip"));

            EfficiencyEntry = cfg.DefineEnum(
                "term_efficiency",
                EfficiencyTerm.EnergySave,
                section,
                () => ModEntry.L("cfg_eff"),
                () => ModEntry.L("cfg_eff_tip"));

            QualityEntry = cfg.DefineEnum(
                "term_quality",
                QualityTerm.Grade,
                section,
                () => ModEntry.L("cfg_qual"),
                () => ModEntry.L("cfg_qual_tip"));

            SuffixEntry = cfg.DefineEnum(
                "name_suffix",
                SuffixMode.None,
                section,
                () => ModEntry.L("cfg_suffix"),
                () => ModEntry.L("cfg_suffix_tip"));
        }
        #endregion
    }
}
