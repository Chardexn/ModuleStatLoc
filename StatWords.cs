using System;
using System.Collections.Generic;

namespace ModuleStatLoc
{
    /// <summary>
    /// 三项模组属性的当前译名，以及说明书正文里的原词替换。
    /// </summary>
    internal static class StatWords
    {
        // 原版词。游戏若把这三个字改成别的，只改这里。
        public const string VanillaPerformance = "性能";
        public const string VanillaEfficiency = "效率";
        public const string VanillaQuality = "质量";
        public const string ModuleImpactMarker = "[模组影响]";

        private static string cachedLocale;
        private static bool isChinese;

        #region MARK: 当前译名
        public static string Performance => Pick(StatLocConfig.PerformanceKeys, (int)StatLocConfig.Performance);

        public static string Efficiency => Pick(StatLocConfig.EfficiencyKeys, (int)StatLocConfig.Efficiency);

        public static string Quality => Pick(StatLocConfig.QualityKeys, (int)StatLocConfig.Quality);

        public static string ForMechanicKey(string key)
        {
            switch (key)
            {
                case "mech_module_stat_performance":
                    return Performance;
                case "mech_module_stat_efficiency":
                    return Efficiency;
                case "mech_module_stat_quality":
                    return Quality;
                default:
                    return key;
            }
        }
        #endregion

        #region MARK: 正文替换
        public static string Rewrite(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return text
                .Replace(VanillaPerformance, "\u0001")
                .Replace(VanillaEfficiency, "\u0002")
                .Replace(VanillaQuality, "\u0003")
                .Replace("\u0001", Performance)
                .Replace("\u0002", Efficiency)
                .Replace("\u0003", Quality);
        }

        public static bool LooksLikeModuleImpact(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            return text.IndexOf(ModuleImpactMarker, StringComparison.Ordinal) >= 0
                || text.IndexOf("[Module Impact]", StringComparison.Ordinal) >= 0;
        }
        #endregion

        #region MARK: 语言
        public static bool IsChineseLocale()
        {
            string code = LocHelper.GetCurrentLocaleCode();
            if (code != cachedLocale)
            {
                cachedLocale = code;
                isChinese = !string.IsNullOrEmpty(code)
                    && code.StartsWith("zh", StringComparison.Ordinal);
            }

            return isChinese;
        }
        #endregion

        #region MARK: 内部
        private static string Pick(IReadOnlyList<string> keys, int index)
        {
            if (index < 0 || index >= keys.Count)
            {
                index = 0;
            }

            return ModEntry.L(keys[index]);
        }
        #endregion
    }
}
